﻿using System.Security.Claims;
namespace SelfEduNet.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static string GetUserId(this ClaimsPrincipal user)
		{
			return user.FindFirstValue(ClaimTypes.NameIdentifier);
		}
		public static string GetUserEmail(this ClaimsPrincipal user)
		{
			return user.FindFirstValue(ClaimTypes.Email);
		}
	}
}
