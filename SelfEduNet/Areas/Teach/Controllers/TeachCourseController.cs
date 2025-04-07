using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Teach.Controllers
{
	[Area("Teach")]
	[Authorize]
	public class TeachCourseController(ICourseService courseService, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor) : Controller
	{
		private readonly ICourseService _courseService = courseService;
		private readonly UserManager<AppUser> _userManager = userManager;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

		//public IActionResult Index()
		//{
		//	return View();
		//}
		
		public async Task<IActionResult> CreateCourse()
		{
			string userId = _httpContextAccessor.HttpContext.User.GetUserId();
			var courseVM = new CreateCourseViewModel() { OwnerId = userId };

			return View(courseVM);
		}
		public async Task<IActionResult> EditContent(int id)
		{
			var courseVM = await _courseService.GetCourseContentViewModelAsync(id);
			return View(courseVM);
		}

		[HttpPost]
		public async Task<IActionResult> CreateModule(int courseId)
		{
			bool result = await _courseService.CreateModuleAsync(courseId);

			return result
				? Ok(new { message = "Модуль створено" })
				: BadRequest(new { message = "Помилка при ствроенні модуля" });
		}

		[HttpPost]
		public async Task<IActionResult> SaveModules([FromBody] List<CourseModules> modules)
		{
			if (modules == null || !modules.Any())
			{
				return BadRequest(new { message = "Нема данних для збереження" });
			}
			ModelState.Clear();
			for (int i = 0; i < modules.Count; i++)
			{
				if (!TryValidateModel(modules[i]))
				{
					return BadRequest(new
					{
						message = $"Перевірте введені дані в модулі {(string.IsNullOrEmpty(modules[i].Title) ? i : modules[i].Title)}",
						error_module_index = modules[i].Id
					});
				}

				await _courseService.UpdateModuleAsync(modules[i]);
			}
			return Ok(new { message = "Всі модулі збережено" });
		}
		//TODO перенести в lessonController
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
		//TODO перенести в lessonController
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
		[HttpPost]
		public async Task<IActionResult> DeleteModule(int moduleId)
		{
			var module = await _courseService.GetModuleByIdAsync(moduleId);

			if (module != null)
			{
				bool result = _courseService.DeleteModule(module);
				return result
					? Ok(new { message = "Модуль видалено" })
					: BadRequest(new { message = "Сталася помилка при видалені модуля." });
			}
			else
			{
				return BadRequest(new { message = "Сталася помилка при видалені модуля. Модуль не знайдено." });
			}
		}
		[HttpPost]
		public async Task<IActionResult> CreateCourse(CreateCourseViewModel courseVM)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Виникла помилка при створенні");
				return View("CreateCourse", courseVM);
			}

			var result = await _courseService.CreateCourseAsync(courseVM);
			if (!result)
			{
				ModelState.AddModelError("", "Курс не знайдено або виникла помилка");
				return View("CreateCourse", courseVM);
			}

			TempData["NotifyMessage"] = "Курс створено";
			TempData["NotifyType"] = "success";

			return RedirectToAction("TeacherCourseList", "Home");
		}
		[HttpPost]
		public async Task<IActionResult> UpdateModuleOrder([FromBody] List<ModuleOrderViewModel> moduleOrder)
		{
			if (moduleOrder == null || !moduleOrder.Any())
				return BadRequest(new { message = "Неправильний формат даних!" });

			await _courseService.UpdateModuleOrderAsync(moduleOrder);

			return Ok(new { message = "Порядок модулів оновлено!" });
		}

		public async Task<IActionResult> RenderModulesList(int courseId)
		{
			var courseVM = await _courseService.GetCourseContentViewModelAsync(courseId);

			return PartialView("_TeachAllModules", courseVM);
		}
	}
}
