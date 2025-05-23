﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SelfEduNet.Data.Enum;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Teach.Controllers
{
	[Area("Teach")]
	[Authorize]
	public class LessonController(ICourseService courseService, IStepService stepService, IUserStepService userStepService,
		IUserLessonService userLessonService, IUserCourseService userCourseService, IMemoryCache cache) : Controller
	{
		private readonly ICourseService _courseService = courseService;
		private readonly IStepService _stepService = stepService;
		private readonly IUserStepService _userStepService = userStepService;
		private readonly IUserLessonService _userLessonService = userLessonService;
		private readonly IUserCourseService _userCourseService = userCourseService;
		private readonly IMemoryCache _cache = cache;

		public IActionResult Index()
		{
			return View("EditLesson");
		}

		[HttpGet("EditLesson/{courseId}/{lessonId?}")]
		public async Task<IActionResult> EditLesson(int courseId, int? lessonId, int? stepId)
		{
			var courseVM = await _courseService.GetCourseContentViewModelAsync(courseId);
			var lesson = lessonId != null
				? courseVM.Lessons.FirstOrDefault(l => l.Id == lessonId)
				: courseVM.Lessons.OrderBy(l => l.Order).FirstOrDefault();
			var steps = await _stepService.GetStepsByLessonIdAsync(lesson.Id, null);

			var editLessonVM = new EditLessonViewModel
			{
				Course = courseVM,
				Lesson = lesson,
				Steps = steps
			};

			ViewData["Lesson"] = lessonId;
			ViewData["Step"] = stepId != null
				? steps.Find(s => s.Id == stepId)
				: steps.FirstOrDefault();

			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Если это AJAX-запрос
			{
				return PartialView("EditLessonMode/_EditLessonContent", editLessonVM);
			}
			return View("EditLesson", editLessonVM);
		}

		[HttpPost]
		public async Task<IActionResult> AddStep(int lessonId, StepType stepType = StepType.Text)
		{
			int maxOrder = await _stepService.GetMaxOrderAsync(lessonId);
			Step step = new Step
			{
				LessonId = lessonId,
				Content = "Новий крок",
				StepType = stepType,
				Order = maxOrder + 1
			};
			bool result = await _stepService.CreateStepAsync(step);
			return result
				? Ok(new { message = "Крок додано", stepId = step.Id })
				: BadRequest(new { message = "Помилка при ствроенні крока" });

		}
		[HttpGet("Lesson/{courseId}/{lessonId?}")]
		public async Task<IActionResult> ViewLesson(int courseId, int? lessonId, int? stepId)
		{
			var userId = User.GetUserId();
			string cacheKey = $"ViewLesson_{userId}_{courseId}_{lessonId}_{stepId}";

			if (!_cache.TryGetValue(cacheKey, out EditLessonViewModel lessonVM))
			{
				var courseVM = await _cache.GetOrCreateAsync(
					$"CourseContent_{courseId}",
					entry => _courseService.GetCourseContentViewModelAsync(courseId)
				);

				var lesson = lessonId.HasValue
					? await _cache.GetOrCreateAsync(
						$"Lesson_{lessonId}_{userId}",
						entry => _courseService.GetLessonByIdAsync(lessonId, userId)
					)
					: courseVM.Lessons.OrderBy(l => l.Order).FirstOrDefault();

				if (lesson == null) return NotFound();

				if (!lesson.UserLessons.Any())
					await _userLessonService.GetOrCreateUserLessonAsync(userId, lesson.Id);

				var allSteps = await _cache.GetOrCreateAsync(
					$"AllSteps_{lesson.Id}",
					entry => _stepService.GetStepsByLessonIdAsync(lesson.Id, null)
				);

				var userSteps = await _cache.GetOrCreateAsync(
					$"UserSteps_{lesson.Id}_{userId}",
					entry => _stepService.GetStepsByLessonIdAsync(lesson.Id, userId)
				);

				if (!userSteps.Any() || userSteps.Count != allSteps.Count)
				{
					userSteps = new List<Step>();
					foreach (var step in allSteps)
					{
						await _userStepService.GetOrCreateUserStepAsync(userId, step.Id);
						userSteps.Add(step);
					}
				}

				Step currentStep = stepId.HasValue
					? await _stepService.GetStepByIdAsync(stepId.Value, userId)
					: userSteps.FirstOrDefault();

				lessonVM = new EditLessonViewModel
				{
					Course = courseVM,
					Lesson = lesson,
					Steps = userSteps
				};

				// Cache for 5 minutes (adjust as needed)
				_cache.Set(cacheKey, lessonVM, TimeSpan.FromMinutes(5));
				ViewData["Lesson"] = lesson.Id;
				ViewData["Step"] = currentStep;
			}
			else
			{
				ViewData["Lesson"] = lessonVM.Lesson.Id;
				ViewData["Step"] = stepId.HasValue
					? lessonVM.Steps.FirstOrDefault(s => s.Id == stepId.Value)
					: lessonVM.Steps.FirstOrDefault();
			}

			return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
				? PartialView("ViewLessonMode/_ViewLessonContent", lessonVM)
				: PartialView("ViewLesson", lessonVM);
		}

		public async Task<IActionResult> CompleteLesson(int courseId, int lessonId, int stepId)
		{
			var userId = User.GetUserId();
			if (!await _userStepService.StepExistsAsync(userId, stepId))
			{
				TempData["NotifyMessage"] = "Крок не знайдено.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });
			}
			var userStep = await _userStepService.GetOrCreateUserStepAsync(userId, stepId);
			if (userStep.Step.StepType == StepType.Test && userStep.UserTestResult is not { IsPassed: true })
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Спочатку необхідно пройти тест";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });
			}

			bool completeStep = await _userStepService.MarkStepAsCompletedAsync(userId, stepId);
			if (!completeStep)
			{
				TempData["NotifyMessage"] = "Помилка при проходженні кроку.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });
			}
			bool isLastStep = await _stepService.IsLastStepInLesson(lessonId, stepId);
			if (!isLastStep)
			{
				TempData["NotifyMessage"] = "Сталася помилка при завершені уроку. Це не останній крок.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });
			}

			var lessonStatistics = await _courseService.GetLessonStatisticsByIdAsync(lessonId, userId);
			if (lessonStatistics.CompletedStepsPercentage < 80)
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Пройдіть хоча б 80% кроків.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });
			}

			bool completeLesson = await _userLessonService.MarkLessonAsCompletedAsync(userId, lessonId);
			if (!completeLesson)
			{
				TempData["NotifyMessage"] = "Помилка при проходженні уроку.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });
			}
			var nextLesson = await _courseService.GetNextLessonAsync(courseId, lessonId);
			if (nextLesson != null)
			{
				TempData["NotifyType"] = "success";
				TempData["NotifyMessage"] = "Урок завершено.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = nextLesson.Id });
			}
			else
			{
				var courseStatistics = await _courseService.GetCourseStatisticsByLessons(courseId, userId);
				if (courseStatistics.CompletedLessonsPercentage < 80)
				{
					TempData["NotifyType"] = "danger";
					TempData["NotifyMessage"] = "Пройдіть хоча б 80% уроків.";
					return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });
				}

				bool result = await _userCourseService.MarkCourseAsCompletedAsync(userId, courseId);
				if (!result)
				{
					TempData["NotifyType"] = "danger";
					TempData["NotifyMessage"] = "Сталася помилка при проходженні курсу.";
					return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId, stepId = stepId });

				}
				TempData["NotifyType"] = "success";
				TempData["NotifyMessage"] = "Вітаємо з проходженням курсу";
				return RedirectToAction("CompletedCourses", "Learning", new { area = "Learn" });
			}
		}
		[HttpPost]
		public async Task<IActionResult> CreateUpdateLesson(int moduleId, int? lessonId, string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				return BadRequest(new { message = "Введіть коректну назву урока" });
			}

			bool result = false;

			if (lessonId.HasValue)
			{
				var lesson = await _courseService.GetLessonByIdAsync(lessonId);

				lesson.Title = title;
				result = await _courseService.UpdateLessonAsync(lesson);
				return result
					? Ok(new { message = "Урок оновлено" })
					: BadRequest(new { message = "Сталася помилка при оновлені уроку." });
			}
			else
			{
				result = await _courseService.CreateLessonAsync(moduleId, title);
				return result
					? Ok(new { message = "Урок створено" })
					: BadRequest(new { message = "Сталася помилка при створені уроку." });
			}
		}
		[HttpPost]
		public async Task<IActionResult> DeleteLesson(int lessonId)
		{
			var lesson = await _courseService.GetLessonByIdAsync(lessonId);

			if (lesson != null)
			{
				bool result = _courseService.DeleteLesson(lesson);
				return result
					? Ok(new { message = "Урок видалено" })
					: BadRequest(new { message = "Сталася помилка при видалені уроку." });
			}
			else
			{
				return BadRequest(new { message = "Сталася помилка при видалені уроку. Урок не знайдено." });
			}
		}
	}
}
