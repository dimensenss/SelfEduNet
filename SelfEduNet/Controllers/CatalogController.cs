
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Controllers
{
    public class CatalogController : Controller
    {
	    private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CatalogController(ICourseRepository courseRepository, ICategoryRepository categoryRepository)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var dto = new CourseBrowseViewModel
            {
                Courses = await _courseRepository.GetAllCoursesAsync(),
                Categories = await _categoryRepository.GetAllCategoriesAsync(),
                CourseFilter = new CourseFilter()
            };
            
            return View(dto);
        }

		[HttpGet]
		public IActionResult SearchCatalog([FromQuery] CourseFilter filter)
		{
			var query = _courseRepository.GetAllCoursesQuery().ApplyFilters(filter);

			int totalItems = query.Count();
			var courses = query
				.Skip((filter.Page - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ToList();

			var viewModel = new CourseCatalogViewModel
			{
				Courses = courses,
				Filter = filter,
				TotalItems = totalItems
			};

			return View(viewModel);
		}
		public async Task<IActionResult> GetCoursesSlider([FromQuery] int catId)
        {
	        var courses = await _courseRepository.GetCoursesByCategoryAsync(catId);

	        ViewData["catId"] = catId;
	        return PartialView("_CoursesSlider", courses);
        }

        [HttpGet]
		public IActionResult GetCoursesWithFilter([FromQuery] CourseFilter filter)
        {
			var query = _courseRepository.GetAllCoursesQuery().ApplyFilters(filter);

			int totalItems = query.Count();
			var courses = query
				.Skip((filter.Page - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ToList();

			var viewModel = new CourseCatalogViewModel
			{
				Courses = courses,
				Filter = filter,
				TotalItems = totalItems
			};
			return PartialView("_GetCoursesWithFilter", viewModel);
        }
	}
}
