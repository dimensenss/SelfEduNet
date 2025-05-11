using EduProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Controllers
{
    public class CourseController(ICourseService courseService, UserManager<AppUser> userManager, IPhotoService photoService, ICategoryService categoryService,
	    IUserCourseService userCourseService) : Controller
    {
	    private readonly ICourseService _courseService = courseService;
	    private readonly UserManager<AppUser> _userManager = userManager;
	    private readonly IPhotoService _photoService = photoService;
	    private readonly ICategoryService _categoryService = categoryService;
	    private readonly IUserCourseService _userCourseService = userCourseService;

	    public async Task<IActionResult> Detail(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            var userId = User.GetUserId();

			var courseVm = new CourseWithUserViewModel()
            {
	            Course = course,
	            UserCourse = userId != null ? await _userCourseService.GetOrCreateUserCourseAsync(userId, id) : null
            };
            return View(courseVm);
        }

		public async Task<IActionResult> EditInfo(int id)
		{
			var courseVM = await _courseService.GetEditViewModelAsync(id);
			if (courseVM == null) 
				return NotFound();

			ViewData["Categories"] = await _categoryService.GetAllCategoriesAsync();
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

			TempData["NotifyType"] = "success";
			TempData["NotifyMessage"] = "Курс успішно оновлено";

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

			if (course.Info.Authors.All(a => a.Id != user.Id))
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
				.Where(u =>
					u.FirstName.ToLower().Contains(name.ToLower()) ||
					u.LastName.ToLower().Contains(name.ToLower()))
				.Select(u => new { id = u.Id, text = u.FirstName + " " + u.LastName })
				.ToListAsync();

			return Json(new { results = users });
		}

		[HttpGet]
		public async Task<IActionResult> SearchCategory(string title)
		{
			if (string.IsNullOrEmpty(title))
			{
				return Json(new { results = new object() });
			}

			var categories = await _categoryService.GetAllCategoriesAsync();
			var filteredCategories = categories
				.Where(c => c.Title.ToLower().Contains(title.ToLower()))
				.Select(c => new { id = c.Id, text = c.Title })
				.ToList();

			return Json(new { results = filteredCategories });
		}
		[HttpPost]
		public async Task<IActionResult> AddCategoryToCourse(int courseId, int categoryId)
		{
			if (categoryId <= 0)
			{
				return BadRequest(new { message = "Щось пішло не так." });
			}
			var course = await _courseService.GetCourseWithInfoByIdAsync(courseId);

			if (course == null)
			{
				return NotFound(new { message = "Курс не знайдено." });
			}

			var category = await _categoryService.GetCategoryByIdAsync(categoryId);
			if (category == null)
			{
				return NotFound(new { message = "Категорію не знайдено." });
			}

			if (course.Category.Id == categoryId)
			{
				return Conflict(new { message = "Цей курс вже знаходиться в цій категорії." });
			}

			course.Category = category;
			_courseService.Update(course);

			return Ok(new { message = "Категорію додано!" });
		}
		public async Task<IActionResult> RenderCategory(int courseId)
		{
			var course = await _courseService.GetCourseWithInfoByIdAsync(courseId);
			var courseVM = new EditCourseViewModel
			{
				Id = course.Id,
				Category = course.Category
			};
			return PartialView("_CategoryContainer", courseVM);
		}
		[HttpPost]
		public async Task<IActionResult> DeleteCourseCategory(int courseId, int categoryId)
		{
			if (categoryId <= 0)
			{
				return BadRequest(new { message = "Щось пішло не так." });
			}
			var course = await _courseService.GetCourseWithInfoByIdAsync(courseId);

			if (course == null)
			{
				return NotFound(new { message = "Курс не знайдено." });
			}

			var category = await _categoryService.GetCategoryByIdAsync(categoryId);
			if (category == null)
			{
				return NotFound(new { message = "Категорію не знайдено." });
			}

			if (course.Category.Id != categoryId)
			{
				return Conflict(new { message = "Цей курс не знаходиться в цій категорії." });
			}
			var draftsCategory = await _categoryService.GetOrCreateDraftsCategoryAsync();
			course.Category = draftsCategory;
			_courseService.Update(course);

			return Ok(new { message = "Категорію видалено. Курс перенесено в чорнетки." });
		}

		[HttpPost]
		public async Task<IActionResult> AddCourseAuthor(int courseId, string authorId)
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

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> SignUpToCourse(int id)
		{
			var course = await _courseService.GetCourseByIdAsync(id);

			string userId = User.GetUserId();
			var userCourse = await _userCourseService.GetOrCreateUserCourseAsync(userId, id);

			var courseDetailVM = new CourseWithUserViewModel()
			{
				Course = course,
				UserCourse = userCourse
			};
			if (course == null)
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Курс не знайдено";
				return View("Detail", courseDetailVM);
			}
			
			if (userId == null)
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Користувача не знайдено";
				return View("Detail", courseDetailVM);
			}

			
			if (userCourse.IsEnrolled)
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Ви вже записані на цей курс.";
				return View("Detail", courseDetailVM);
			}

			await _userCourseService.MarkCourseAsEnrolledAsync(userId, id);

			return Redirect($"/Lesson/{course.Id}");
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddOrDeleteCourseToWish(int id, bool delete = false)
		{
			var course = await _courseService.GetCourseByIdAsync(id);

			string userId = User.GetUserId();
			var userCourse = await _userCourseService.GetOrCreateUserCourseAsync(userId, id);

			var courseDetailVM = new CourseWithUserViewModel()
			{
				Course = course,
				UserCourse = userCourse
			};
			if (course == null)
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Курс не знайдено";
				return View("Detail", courseDetailVM);
			}

			if (userId == null)
			{
				TempData["NotifyType"] = "danger";
				TempData["NotifyMessage"] = "Користувача не знайдено";
				return View("Detail", courseDetailVM);
			}

			if (!delete)
			{
				userCourse.IsWish = true;
				_userCourseService.Update(userCourse);
			}
			else
			{
				userCourse.IsWish = false;
				_userCourseService.Update(userCourse);
			}

			courseDetailVM.UserCourse = userCourse;

			return RedirectToAction("Detail", new { id = course.Id });
		}
	}
}
