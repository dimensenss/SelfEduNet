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

	public async Task<IActionResult> CreateReviewForm(int id)
	{
		string userId = User.GetUserId();
		var userCourse = await _userCourseService.GetOrCreateUserCourseAsync(userId, id);

		string redirectUrl = Url.Action("CompletedCourses", "Learning", new { area = "Learn" });

		if (userCourse.IsEnrolled == false)
		{
			return BadRequest(new
			{
				message = "Ви не можете залишити відгук на курс, якщо ви не пройшли його.",
				redirectUrl = redirectUrl
			});
		}
		if (userCourse.Review != null)
		{
			return BadRequest(new
			{
				message = "Ви не можете залишити більше ніж один відгук",
				redirectUrl = redirectUrl
			});
		}

		return PartialView("_CreateReviewForm", userCourse);
	}
	[HttpPost]
	public async Task<IActionResult> CreateReview(CourseReviewViewModel courseReviewVM)
	{
		string userId = User.GetUserId();

		if (!ModelState.IsValid)
		{
			TempData["NotifyMessage"] = "Відгук не збережено. Перевірте правильність заповнення форми.";
			TempData["NotifyType"] = "danger";

			return RedirectToAction("CompletedCourses");
		}

		var userCourse = await _userCourseService.GetOrCreateUserCourseAsync(userId, courseReviewVM.CourseId);
		if (userCourse == null || !userCourse.IsEnrolled)
		{
			TempData["NotifyMessage"] = "Ви не можете залишити відгук на курс, якщо ви не пройшли його";
			TempData["NotifyType"] = "danger";

			return RedirectToAction("CompletedCourses");
		}

		var review = new Review
		{
			UserCourseId = userCourse.Id,
			UserCourse = userCourse,
			UserId = userId,
			Text = courseReviewVM.Text,
			Rate = courseReviewVM.Rate,
			CreatedAt = DateTime.UtcNow
		};

		bool result = await _userCourseService.CreateReviewAsync(review);

		if (!result)
		{
			TempData["NotifyMessage"] = "Відгук не збережено. Спробуйте ще раз.";
			TempData["NotifyType"] = "danger";
			return RedirectToAction("CompletedCourses");
		}
		TempData["NotifyMessage"] = "Відгук успішно збережено!";
		TempData["NotifyType"] = "success";

		return RedirectToAction("CompletedCourses");
	}
}

