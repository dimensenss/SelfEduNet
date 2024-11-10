using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Data;
using SelfEduNet.Models;
using SelfEduNet.Services;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Controllers
{
    public class AccountController(ApplicationDbContext context, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager, IEmailSender emailSenderService) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly IEmailSender _emailSenderService = emailSenderService;

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Ви вийшли з акаунту";
            return RedirectToAction("Index", "Home");
        }
    }
}
