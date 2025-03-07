using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AEET.Models;
using System.Linq;

namespace AEET.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // 1. Query the UserMasters table for a matching record
            var user = _context.UserMasters.FirstOrDefault(u =>
                u.EmailID == username && u.PasswordHash == password);
            
            if (user != null)
            {
                // 2. Retrieve the role name from the RoleMasters table
                var roleName = _context.RoleMasters
                    .FirstOrDefault(r => r.RoleID == user.RoleID)
                    ?.RoleName ?? "";
                
                // 3. Store the user ID and role in session
                // If UserID is a Guid, convert it to string for session
                HttpContext.Session.SetString("UserId", user.UserID.ToString());
                HttpContext.Session.SetString("UserRole", roleName);
                
                // 4. Redirect to Landing page on successful login
                return RedirectToAction("Landing", "Home");
            }
            
            // If no match is found, show an error
            ViewBag.Error = "Invalid username or password.";
            return View();
        }
        
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear session and redirect to the login page
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
