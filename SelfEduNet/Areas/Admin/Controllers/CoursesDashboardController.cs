using CloudinaryDotNet;
using EduProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Areas.Admin.Helpers;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class CoursesDashboardController(ICourseRepository courseRepository, ICategoryRepository categoryRepository, IPhotoService photoService,
		IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, ICourseService courseService) : Controller
    {
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IPhotoService _photoService = photoService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ICourseService _courseService = courseService;

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
    }
}
