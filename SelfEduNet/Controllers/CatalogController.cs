using Microsoft.AspNetCore.Mvc;

namespace SelfEduNet.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
