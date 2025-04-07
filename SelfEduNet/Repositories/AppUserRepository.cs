using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories
{
	public interface IAppUserRepository
	{

	}
	public class AppUserRepository(UserManager<AppUser> userManager)
	{
		private readonly UserManager<AppUser> _userManager = userManager;

	
	}
}
