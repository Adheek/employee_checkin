using Microsoft.AspNetCore.Mvc;
using AEET.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AEET.Controllers
{
    public class ManageUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManageUsers/Index
        public IActionResult Index()
        {
            // Optionally, load a list of existing users to display
            var users = _context.UserMasters.ToList();
            return View(users);
        }


 [HttpGet]
        public IActionResult GetUserList()
        {
            // Query the UserMaster table and select only the columns you need
            var userList = _context.UserMasters
                .Select(u => new 
                {
                    u.Username,
                    u.EmployeeName,
                    Email = u.EmailID,
                    RoleName = u.Role.RoleName, // using the navigation property for Role
                    u.Location
                })
                .ToList();

            // Return the result as JSON
            return Json(userList);
        }

        // POST: ManageUsers/AddUser
        [HttpPost]
        public IActionResult AddUser(UserMaster model)
        {
            if (ModelState.IsValid)
            {
                // Map the role dropdown value to a RoleID.
                // Expecting the dropdown value from the view to be one of:
                // "SA" for Super Admin User, "AA Team" for Asset Admin, "TO" for Transaction Manager.
                string? roleDropdown = Request.Form["role"];
                Guid roleId;
                switch (roleDropdown)
                {
                    case "SA":
                        roleId = _context.RoleMasters.FirstOrDefault(r => r.RoleName == "Super Admin")?.RoleID ?? Guid.Empty;
                        break;
                    case "AA Team":
                        roleId = _context.RoleMasters.FirstOrDefault(r => r.RoleName == "Asset Administrator")?.RoleID ?? Guid.Empty;
                        break;
                    case "TO":
                        roleId = _context.RoleMasters.FirstOrDefault(r => r.RoleName == "Transaction Manager")?.RoleID ?? Guid.Empty;
                        break;
                    default:
                        roleId = Guid.Empty;
                        break;
                }
                if (roleId == Guid.Empty)
                {
                    ModelState.AddModelError("role", "Invalid role selected.");
                    return View("Index", _context.UserMasters.ToList());
                }
                model.RoleID = roleId;

                // Set the creation properties
                model.UserID = Guid.NewGuid();  // Generate a new unique ID
                model.CreatedOn = DateTime.Now;
                model.CreatedBy = "system"; // Or the logged in admin's username

                // Optionally, you could hash the password here before saving

                // Add the new user to the database
                _context.UserMasters.Add(model);
                _context.SaveChanges();

                // Redirect back to the index view after success
                return RedirectToAction("Index");
            }
            // If model validation fails, redisplay the view with errors
            return View("Index", _context.UserMasters.ToList());
        }
    }
}
