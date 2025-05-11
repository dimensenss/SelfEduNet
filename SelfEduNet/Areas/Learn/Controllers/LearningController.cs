using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Areas.Learn.Controllers;

[Area("Learn")]
[Authorize]
public class LearningController(IUserCourseService userCourseService) : Controller
{
	private readonly IUserCourseService _userCourseService = userCourseService;

	public async Task<IActionResult> Index()
    {
		string userId = User.GetUserId();
		var userCoursesCompletion = await _userCourseService.GetAllUserCoursesWithCompletionAsync(userId);

		var enrolledCourses = new LearningViewModel()
	    {
		    EnrolledCourses = userCoursesCompletion
		};

		return View(enrolledCourses);
    }
	public async Task<IActionResult> ActiveCourses()
	{
		string userId = User.GetUserId();
		var userCourses = await _userCourseService.GetAllUserCoursesAsync(userId);

		
		return View(userCourses);
	}
	public async Task<IActionResult> RenderUserCoursesList(string query)
	{
		string userId = User.GetUserId();
		var userCourses = await _userCourseService.GetAllUserCoursesByNameAsync(userId, query ?? "");

		return PartialView("_GetActiveCourses", userCourses);
	}
	public async Task<IActionResult> CompletedCourses()
	{
		string userId = User.GetUserId();
		var completedUserCourses = await _userCourseService.GetAllCompletedUserCoursesAsync(userId);

		var completedCourses = new LearningViewModel()
		{
			EnrolledCourses = completedUserCourses
		};

		return View(completedCourses);
	}

	public async Task<IActionResult> WishedCourses()
	{
		string userId = User.GetUserId();
		var wishedCourses = await _userCourseService.GetWishedUserCoursesAsync(userId);

		return View(wishedCourses);
	}
}

