using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AEET.Controllers
{
    public class DashboardController : Controller
    {
        // API to get user role
        [HttpGet]
        public IActionResult GetUserRole()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != null)
            {
                return Json(new { role = userRole });
            }
            return Json(new { role = "" });
        }
    }
}
