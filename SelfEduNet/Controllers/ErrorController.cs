using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Models;
using System.Diagnostics;

namespace SelfEduNet.Controllers
{
	public class ErrorController : Controller
	{
		[Route("Error/{statusCode}")]
		public IActionResult HttpStatusCodeHandler(int statusCode)
		{
			switch (statusCode)
			{
				case 404:
					return View("NotFound");
				default:
					return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}
		}
	}
}
