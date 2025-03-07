using Microsoft.AspNetCore.Mvc;

namespace AEET.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Landing()
        {
            return View();
        }
    }
}
