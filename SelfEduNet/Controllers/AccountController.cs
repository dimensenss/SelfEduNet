using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Data;
using SelfEduNet.Models;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Controllers
{
    public class AccountController(ApplicationDbContext context, UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        public IActionResult Login()
        {
            var loginViewModel = new AccountLoginViewModel();
            return View(loginViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginViewModel accountLoginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(accountLoginViewModel);
            }

            var user = await _userManager.FindByEmailAsync(accountLoginViewModel.EmailAddress);
            if (user == null)
            {
                TempData["Errors"] = "Неправильний email або пароль, спробуйте ще раз.";
                return View(accountLoginViewModel);
            }

            bool checkPassword = await _userManager.CheckPasswordAsync(user, accountLoginViewModel.Password);

            if (!checkPassword)
            {
                TempData["Errors"] = "Неправильний email або пароль, спробуйте ще раз.";
                return View(accountLoginViewModel);
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, accountLoginViewModel.Password, false, false);
            if (!signInResult.Succeeded)
            {
                TempData["Error"] = "Неправильний email або пароль, спробуйте ще раз.";
                return View(accountLoginViewModel);
                
            }

            TempData["SuccessMessage"] = $"Ви успішно увійшли в акаунт";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            var registrationViewModel = new AccountRegistrationViewModel();
            return View(registrationViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Registration(AccountRegistrationViewModel accountRegistrationViewModel)
        {
	        if (!ModelState.IsValid)
	        {
		        return View(accountRegistrationViewModel);
	        }

	        var user = await _userManager.FindByEmailAsync(accountRegistrationViewModel.EmailAddress);
	        if (user != null)
	        {
		        TempData["Errors"] = "Користувач з таким email вже існує";
		        return View(accountRegistrationViewModel);
	        }

	        var newUser = new AppUser
	        {
		        Email = accountRegistrationViewModel.EmailAddress,
		        UserName = accountRegistrationViewModel.EmailAddress
	        };

	        var newUserResult = await _userManager.CreateAsync(newUser, accountRegistrationViewModel.Password);

	        if (!newUserResult.Succeeded)
	        {
		        var errors = string.Join(", ", newUserResult.Errors.Select(e => e.Description));
		        TempData["Error"] = errors;
		        return View(accountRegistrationViewModel);
	        }

	        await _userManager.AddToRoleAsync(newUser, UserRoles.User);
	        await _signInManager.PasswordSignInAsync(newUser, accountRegistrationViewModel.Password, false, false);

	        TempData["SuccessMessage"] = "Ваш акаунт зарєестровано";
			return RedirectToAction("Index", "Home");
		}






		public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
