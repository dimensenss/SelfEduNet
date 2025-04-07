using EduProject.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Controllers
{
    public class CourseController(ICourseService courseService, UserManager<AppUser> userManager, IPhotoService photoService) : Controller
    {
	    private readonly ICourseService _courseService = courseService;
	    private readonly UserManager<AppUser> _userManager = userManager;
	    private readonly IPhotoService _photoService = photoService;

	    public async Task<IActionResult> Detail(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return View(course);
        }

		public async Task<IActionResult> EditInfo(int id)
		{
			var courseVM = await _courseService.GetEditViewModelAsync(id);
			if (courseVM == null) return NotFound();

			//ViewData["Categories"] = await _categoryRepository.GetAllCategoriesAsync();
			return View(courseVM);
		}
		[HttpPost]
		public async Task<IActionResult> EditInfo(int id, EditCourseViewModel courseVM)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Виникла помилка при редагуванні");
				return View("EditInfo", courseVM);
			}

			var result = await _courseService.UpdateCourseAsync(id, courseVM);
			if (!result)
			{
				ModelState.AddModelError("", "Курс не знайдено або виникла помилка");
				return View("EditInfo", courseVM);
			}
			var previousUrl = Request.Headers["Referer"].ToString();

			return Redirect(previousUrl);
		}
		public async Task<IActionResult> EditInfoAndContinueEdit(int id, EditCourseViewModel courseVM)
		{
			if (!ModelState.IsValid)
			{
				return Json(new { success = false, message = "Виникла помилка при редагуванні", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
			}

			var result = await _courseService.UpdateCourseAsync(id, courseVM);
			if (!result)
			{
				return Json(new { success = false, message = "Курс не знайдено або виникла помилка" });
			}

			return Json(new { success = true, message = "Курс успішно оновлено" });
		}

		public async Task<IActionResult> UpdatePromoText(int id, [FromBody] string content)
		{
			if (content == null)
			{
				return BadRequest();
			}

			var course = await _courseService.GetCourseByIdAsync(id);
			if (course == null)
			{
				return NotFound(new { message = "Курс не знайдено" });
			}
			//var sanitizedContent = _sanitizer.Sanitize(content);

			var imageUrls = _photoService.ExtractImageUrls(content);
			var oldImageUrls = _photoService.ExtractImageUrls(course.PromoText ?? "");
			foreach (var oldImageUrl in oldImageUrls.Except(imageUrls))
			{
				try
				{
					var fileInfo = new FileInfo(oldImageUrl);
					string publicId = Path.GetFileNameWithoutExtension(fileInfo.Name);
					await _photoService.DeleteFileAsync(publicId);
				}
				catch
				{
					// Логируем ошибку или игнорируем
				}
			}

			course.PromoText = content;
			course.UpdatedAt = DateTime.UtcNow;
			bool result = _courseService.Update(course);

			return result
				? Ok()
				: BadRequest();
		}

		[HttpPost]
		public async Task<IActionResult> DeleteCourse(int courseId)
		{
			 var course = await _courseService.GetCourseByIdAsync(courseId);
			if (course == null)
			{
				return NotFound(new { message = "Курс не знайдено" });
			}

			_courseService.DeleteCourse(course);

			return Ok(new { message = "Курс видалено!" });
		}

		[HttpPost]
		public async Task<IActionResult> DeleteCourseAuthor(int courseId, string authorId)
		{
			if (string.IsNullOrEmpty(authorId))
			{
				return BadRequest(new { message = "Щось пішло не так." });
			}
			var course = await _courseService.GetCourseWithInfoByIdAsync(courseId);

			if (course == null)
			{
				return NotFound(new { message = "Курс не знайдено." });
			}

			var user = await _userManager.FindByIdAsync(authorId);
			if (user == null)
			{
				return NotFound(new { message = "Користувача не знайдено." });
			}

			if (!course.Info.Authors.Any(a => a.Id == user.Id))
			{
				return Conflict(new { message = "Користувач не є автором цього курсу." });
			}

			course.Info.Authors.Remove(user);
			_courseService.Update(course);

			return Ok(new { message = "Автора видалено!" });
		}

		[HttpGet]
		public async Task<IActionResult> SearchAuthors(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return Json(new { results = new List<object>() });
			}

			// TODO перенести
			var users = await _userManager.Users
				.Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name)) // Замените на поле, которое вы используете для имени
				.Select(u => new { id = u.Id, text = u.FirstName + " " + u.LastName }) // Замените на поля, которые вам нужны
				.ToListAsync();

			return Json(new { results = users });
		}

		[HttpPost]
		public async Task<IActionResult> AddCourseAuthor(int courseId, string authorId)
		{
			//TODO перевести
			if (string.IsNullOrEmpty(authorId))
			{
				return BadRequest(new { message = "Щось пішло не так." });
			}
			var course = await _courseService.GetCourseWithInfoByIdAsync(courseId);

			if (course == null)
			{
				return NotFound(new { message = "Курс не знайдено." });
			}

			var user = await _userManager.FindByIdAsync(authorId);
			if (user == null)
			{
				return NotFound(new { message = "Користувача не знайдено." });
			}

			if (course.Info.Authors.Any(a => a.Id == user.Id))
			{
				return Conflict(new { message = "Цей користувач вже є автором." });
			}

			course.Info.Authors.Add(user);
			_courseService.Update(course);

			return Ok(new { message = "Автора додано!" });
		}


		public async Task<IActionResult> RenderAuthorsList(int courseId)
		{
			var course = await _courseService.GetCourseWithInfoByIdAsync(courseId);
			var courseVM = new EditCourseViewModel
			{
				Id = course.Id,
				Authors = course.Info.Authors
			};
			return PartialView("_AuthorsContainer", courseVM);
		}

		public async Task<IActionResult> RenderCoursesList()
		{
			var courses = await _courseService.GetAllCoursesAsync();
				
			return PartialView("_GetCoursesWithEdit", courses);
		}

		public async Task<IActionResult> RenderTeacherCourses([FromQuery] CourseFilter filter)
		{
			string userId = User.GetUserId();
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return NotFound(new { message = "User not found." });
			}

			var courses = _courseService.GetAllCoursesByOwnerQuery(userId).ApplyFilters(filter);

			return PartialView("_GetCoursesWithEdit", courses);
		}

		[HttpPost]
		public async Task<IActionResult> SignUpToCourse(int id)
		{
			var course = await _courseService.GetCourseByIdAsync(id);
			if (course == null)
			{
				return NotFound(new { message = "Курс не знайдено" });
			}
			var userId = User.GetUserId();

			return Ok();
		}

	}
}
