﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Extensions;
using SelfEduNet.Interfaces;
using SelfEduNet.Models;
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
            var dto = new MainCatalogViewModel
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
            var courses = _courseRepository.GetAllCoursesQuery().ApplyFilters(filter);
            return View(courses);
        }
        public async Task<IActionResult> GetCourses([FromQuery] int catId)
        {
	        var courses = await _courseRepository.GetCoursesByCategoryAsync(catId);

	        ViewData["catId"] = catId;
	        return PartialView("_CoursesSlider", courses);
        }

        [HttpGet]
		public IActionResult SearchCatalogCourseList([FromQuery] CourseFilter filter)
        {
	        var courses = _courseRepository.GetAllCoursesQuery().ApplyFilters(filter);
	        return PartialView("_SearchCatalogCourseList", courses);
        }

	}
}