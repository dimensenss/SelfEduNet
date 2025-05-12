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
	public class TeachCourseController(ICourseService courseService, UserManager<AppUser> userManager) : Controller
	{
		private readonly ICourseService _courseService = courseService;
		private readonly UserManager<AppUser> _userManager = userManager;

		//public IActionResult Index()
		//{
		//	return View();
		//}
		
		public async Task<IActionResult> CreateCourse()
		{
			string userId = User.GetUserId();
			var user = await _userManager.FindByIdAsync(userId);
			if (!await _userManager.IsInRoleAsync(user, "Teacher"))
			{
				await _userManager.AddToRoleAsync(user, "Teacher");
				TempData["NotifyType"] = "success";
				TempData["NotifyMessage"] = "Вітаємо, ви стали вчителем!";
			}
			var courseVM = new CreateCourseViewModel()
			{
				OwnerId = userId,
				Owner = user
			};

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
		public async Task<IActionResult> PublishCourse(int id)
		{
			var courseCheckList = await _courseService.GetCourseChecklistAsync(id);

			return View(courseCheckList);
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
