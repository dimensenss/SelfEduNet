using Microsoft.AspNetCore.Identity;
using SelfEduNet.Models;

namespace SelfEduNet.Data
{
	public static class RoleInitializer
	{
		public static async Task InitializeAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			string[] roleNames = { "Admin", "User", "Teacher" };

			foreach (var roleName in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// Дополнительно: создание администратора, если его нет
			var adminEmail = "root@gmail.com";
			var adminPassword = "Admin@1234";

			if (await userManager.FindByEmailAsync(adminEmail) == null)
			{
				var adminUser = new AppUser { UserName = adminEmail, Email = adminEmail };
				var result = await userManager.CreateAsync(adminUser, adminPassword);

				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "Admin");
				}
			}
		}
	}
}
