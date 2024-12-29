using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Teach.Controllers
{
    [Area("Teach")]
	public class HomeController(UserManager<AppUser> userManager, ICourseRepository courseRepository) : Controller
	{
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ICourseRepository _courseRepository = courseRepository;

        public IActionResult Index()
		{
			return View();
		}

        public async Task<IActionResult> TeacherCourseList()
        {
            string userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var courses = await _courseRepository.GetCoursesByOwnerAsync(userId);

            return View(courses);

        }

        public async Task<IActionResult> RenderTeacherCourses([FromQuery] CourseFilter filter)
        {
	        string userId = User.GetUserId();
	        var user = await _userManager.FindByIdAsync(userId);

	        if (user == null)
	        {
		        return NotFound(new { message = "User not found." });
	        }

	        var courses =  _courseRepository.GetAllCoursesByOwnerQuery(userId).ApplyFilters(filter);

            return PartialView("_GetCoursesWithEdit", courses);
        }
	}
}
