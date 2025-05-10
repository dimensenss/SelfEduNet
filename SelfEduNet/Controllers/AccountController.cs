using CloudinaryDotNet;
using EduProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Controllers;

[Authorize]
public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, 
	IPhotoService photoService) : Controller
{
	private readonly SignInManager<AppUser> _signInManager = signInManager;
	private readonly UserManager<AppUser> _userManager = userManager;
	private readonly IPhotoService _photoService = photoService;

	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();

		TempData["NotifyType"] = "success";
		TempData["NotifyMessage"] = "Ви вийшли з акаунту";

		return RedirectToAction("Index", "Home");
	}

	public async Task<IActionResult> Profile()
	{
		var userId = User.GetUserId();
		var user = await _userManager.FindByIdAsync(userId);

		if (user == null) return NotFound(new { message = "User not found." });

		var userInfo = new EditUserProfileViewModel()
		{
			FirstName = user.FirstName,
			LastName = user.LastName,
			PhotoURL = user.PhotoURL
		};

		return View(userInfo);
	}
	[HttpPost]
	public async Task<IActionResult> Profile(EditUserProfileViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var userId = User.GetUserId();
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return NotFound(new { message = "User not found." });
		}

		string photoURL = model.PhotoURL;
		if (model.PhotoFile != null)
		{
			try
			{
				if (user.PhotoURL != null)
				{
					var fileInfo = new FileInfo(user.PhotoURL);
					string publicId = Path.GetFileNameWithoutExtension(fileInfo.Name);
					await _photoService.DeleteFileAsync(publicId);
				}
			}
			catch
			{
				//TODO : log error
			}

			var resultPhoto = await _photoService.AddPhotoAsync(model.PhotoFile);
			photoURL = resultPhoto.Url.ToString();
		}
		user.FirstName = model.FirstName;
		user.LastName = model.LastName;
		user.PhotoURL = photoURL;

		var result = await _userManager.UpdateAsync(user);

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}
		TempData["NotifyType"] = "success";
		TempData["NotifyMessage"] = "Дані збережено.";

		return RedirectToAction("Profile");
	}
}