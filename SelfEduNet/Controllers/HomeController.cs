using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Models;
using System.Diagnostics;

namespace SelfEduNet.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}


	}
}
