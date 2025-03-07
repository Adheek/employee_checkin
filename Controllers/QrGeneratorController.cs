using Microsoft.AspNetCore.Mvc;

namespace AEET.Controllers
{
    public class QrGeneratorController : Controller
    {
        // GET: /QrGenerator/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}
