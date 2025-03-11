using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AEET.Controllers
{
    public class ManageUsersController : Controller
    {
        public ManageUsersController()
        {
        }


        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
