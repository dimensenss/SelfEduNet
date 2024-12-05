using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Repositories;

namespace SelfEduNet.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public async Task<IActionResult> Detail(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            return View(course);
        }
    }
}
