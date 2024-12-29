using CloudinaryDotNet;
using EduProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Areas.Admin.Helpers;
using SelfEduNet.Areas.Admin.ViewModels;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class CoursesDashboardController(ICourseRepository courseRepository, ICategoryRepository categoryRepository, IPhotoService photoService,
		IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : Controller
    {
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IPhotoService _photoService = photoService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<AppUser> _userManager = userManager;

        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return View(courses);
        }

        public async Task<IActionResult> SearchCourse([FromQuery] CourseFilter filter)
        {
			var courses = _courseRepository.GetAllCoursesQuery().ApplyFilters(filter);

			ViewData["Query"] = filter.Query;
			return View("Index", courses);
		}
        public async Task<IActionResult> EditInfo(int id)
        {
            var course = await _courseRepository.GetCourseWithInfoByIdAsync(id);
            var courseVM = new EditCourseViewModel
            {
	            Id = course.Id,
	            //Category = course.Category,
	            //CategoryId = course.CategoryId,
                PreviewURL = course.Preview,
	            CourseName = course.CourseName,
	            Description = course.Description,
	            Language = course.Language,
	            Difficulty = course.Difficulty,
	            FullPrice = course.FullPrice,
	            HaveCertificate = course.HaveCertificate,
	            Workload = course.Info.Workload,
	            Authors = course.Info.Authors,
	            PreviewVideo = course.Info.PreviewVideo,
	            IsPublished = course.IsPublished
            };
            ViewData["Categories"] = await categoryRepository.GetAllCategoriesAsync();

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

            var course = await _courseRepository.GetCourseWithInfoByIdAsync(id);
            if (course == null)
            {
                return View("EditInfo", courseVM);
            }

            string previewURL = courseVM.PreviewURL;
            if (courseVM.Preview != null)
            {
                try
                {
                    var fileInfo = new FileInfo(course.Preview);
                    string publicId = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    await _photoService.DeletePhotoAsync(publicId);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Виникла помилка при видаленні фото");
                }
                var resultPhoto = await _photoService.AddPhotoAsync(courseVM.Preview);
                previewURL = resultPhoto.Url.ToString();
            }

            var newCourseInfo = new CourseInfo()
            {
                Id = course.Info.Id,
                CourseId = id,
                Course = course,
                Workload = courseVM.Workload,
                PreviewVideo = courseVM.PreviewVideo,
                Authors = courseVM.Authors
            };
            
            //Category = courseVM.Category,
            //CategoryId = courseVM.CategoryId,
            course.Preview = previewURL;
            course.CourseName = courseVM.CourseName;
            course.Description = courseVM.Description;
            course.Language = courseVM.Language;
            course.Difficulty = courseVM.Difficulty;
            course.FullPrice = courseVM.FullPrice;
            course.HaveCertificate = courseVM.HaveCertificate;
            course.Info = newCourseInfo;
            course.IsPublished = courseVM.IsPublished;

            
            _courseRepository.Update(course);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
	        var course = await _courseRepository.GetCourseByIdAsync(courseId);
	        if (course == null)
	        {
		        return NotFound(new { message = "Course not found." });
	        }

	        _courseRepository.Delete(course);

	        return Ok(new { message = "Course successfully deleted!" });
		}

        [HttpPost]
        public async Task<IActionResult> DeleteCourseAuthor(int courseId, string authorId)
        {
			if (string.IsNullOrEmpty(authorId))
			{
				return BadRequest(new { message = "Invalid request." });
			}
			var course = await _courseRepository.GetCourseWithInfoByIdAsync(courseId);

			if (course == null)
			{
				return NotFound(new { message = "Course not found." });
			}

			var user = await _userManager.FindByIdAsync(authorId);
			if (user == null)
			{
				return NotFound(new { message = "User not found." });
			}

			if (!course.Info.Authors.Any(a => a.Id == user.Id))
			{
				return Conflict(new { message = "The user is not an author for this course." });
			}

			course.Info.Authors.Remove(user);
			_courseRepository.Update(course);

			return Ok(new { message = "Author deleted successfully from the course!" });
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
				.Select(u => new { id = u.Id, text = u.FirstName + u.LastName }) // Замените на поля, которые вам нужны
				.ToListAsync();

			return Json(new { results = users });
		}

        [HttpPost]
        public async Task<IActionResult> AddCourseAuthor(int courseId, string authorId)
        {
	        if (string.IsNullOrEmpty(authorId))
	        {
		        return BadRequest(new { message = "Invalid request." });
	        }
	        var course = await _courseRepository.GetCourseWithInfoByIdAsync(courseId);

			if (course == null)
	        {
		        return NotFound(new { message = "Course not found." });
	        }

	        var user = await _userManager.FindByIdAsync(authorId);
	        if (user == null)
	        {
		        return NotFound(new { message = "User not found." });
	        }

	        if (course.Info.Authors.Any(a => a.Id == user.Id))
	        {
		        return Conflict(new { message = "The user is already an author for this course." });
	        }

			course.Info.Authors.Add(user);
	        _courseRepository.Update(course);

			return Ok(new { message = "Author added successfully to the course!" });
        }

        public async Task<IActionResult> RenderAuthorsList(int courseId)
        {
	        var course = await _courseRepository.GetCourseWithInfoByIdAsync(courseId);
	        var courseVM = new EditCourseViewModel
	        {
		        Id = course.Id,
		        Authors = course.Info.Authors
	        };
			return PartialView("_AuthorsContainer", courseVM);
		}

        public async Task<IActionResult> RenderCoursesList()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            
            return PartialView("_GetCoursesWithEdit", courses);
        }
    }
}
