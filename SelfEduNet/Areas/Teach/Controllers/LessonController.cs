using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Data.Enum;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Teach.Controllers
{
	[Area("Teach")]
	public class LessonController(ICourseService courseService, IStepService stepService, IUserStepService userStepService,
		IUserLessonService userLessonService) : Controller
	{
		private readonly ICourseService _courseService = courseService;
		private readonly IStepService _stepService = stepService;
		private readonly IUserStepService _userStepService = userStepService;
		private readonly IUserLessonService _userLessonService = userLessonService;

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
				: steps.Find(s => s.Order == 1);

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
			var courseVM = await _courseService.GetCourseContentViewModelAsync(courseId);
			var lesson = lessonId.HasValue
				? await _courseService.GetLessonByIdAsync(lessonId, userId)
				: courseVM.Lessons.OrderBy(l => l.Order).FirstOrDefault();

			if (lesson == null) return NotFound();

			if (!lesson.UserLessons.Any())
			{
				await _userLessonService.GetOrCreateUserLessonAsync(userId, lesson.Id);
			}

			// Получаем шаги с учетом пользователя
			var allSteps = await _stepService.GetStepsByLessonIdAsync(lesson.Id, null);
			var userSteps = await _stepService.GetStepsByLessonIdAsync(lesson.Id, userId);

			// Если у пользователя нет записей по шагам, загружаем все и создаем UserStep
			if (!userSteps.Any() || userSteps.Count != allSteps.Count())
			{
				userSteps = new List<Step>();

				foreach (var step in allSteps)
				{
					var userStep = await _userStepService.GetOrCreateUserStepAsync(userId, step.Id);
					userSteps.Add(step);
				}
			}

			// Определяем текущий шаг
			Step currentStep = stepId.HasValue
				? await _stepService.GetStepByIdAsync(stepId.Value, userId)
				: userSteps.FirstOrDefault();

			var lessonVM = new EditLessonViewModel
			{
				Course = courseVM,
				Lesson = lesson,
				Steps = userSteps
			};

			ViewData["Lesson"] = lesson.Id;
			ViewData["Step"] = currentStep;

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
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId });
			}

			bool completeStep = await _userStepService.MarkStepAsCompletedAsync(userId, stepId);
			if (!completeStep)
			{
				TempData["NotifyMessage"] = "Помилка при проходженні кроку.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId });
			}
			bool isLastStep = await _stepService.IsLastStepInLesson(lessonId, stepId);
			if (!isLastStep)
			{
				TempData["NotifyMessage"] = "Сталася помилка при завершені уроку. Це не останній крок.";
				return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId });
			}
			var lessonStatistics = await _courseService.GetLessonStatisticsByIdAsync(lessonId, userId);
			if (lessonStatistics.CompletedStepsPercentage >= 80)
			{
				bool completeLesson = await _userLessonService.MarkLessonAsCompletedAsync(userId, lessonId);
				if (!completeLesson)
				{
					TempData["NotifyMessage"] = "Помилка при проходженні урока.";
					return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId });
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
					//check complition of course
					//redirect on certificate
					//update course is completed
					TempData["NotifyType"] = "success";
					TempData["NotifyMessage"] = "Вітаємо з проходженням курсу";
					return Ok("Course completed");
				}
			}
			TempData["NotifyType"] = "danger";
			TempData["NotifyMessage"] = "Пройдіть хоча б 80% кроків."; // count of steps need to be cpompleted
			return RedirectToAction("ViewLesson", new { courseId = courseId, lessonId = lessonId });
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

		//TODO Complete module
		//TODO Complete course

		//public async Task<IActionResult> RenderStepsList(int lessonId)
		//{
		//	var steps = await _stepService.GetStepsByLessonIdAsync(lessonId);
		//	return PartialView("_TeachAllSteps");
		//}
	}
}
