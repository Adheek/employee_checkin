using Microsoft.AspNetCore.Mvc;

namespace AEET.Controllers
{
    public class AddNewAssetController : Controller
    {
        public IActionResult Index()
        {
            return View(); // ✅ Loads Views/AddNewAsset/Index.cshtml
        }
    }
}
