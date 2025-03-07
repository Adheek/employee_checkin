using Microsoft.AspNetCore.Mvc;
using AEET.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AEET.Code;

namespace AEET.Controllers
{
    public class ManageUsersController : Controller
    {

        public ManageUsersController()
        {
        }

        // GET: ManageUsers/Index
        public async Task<IActionResult> Index([FromServices] AEET.Code.UserManager user)
        {
            // Optionally, load a list of existing users to display
            return View(await user.GetUsers());
        }

        // POST: ManageUsers/AddUser
        [HttpPost]
        public async Task<IActionResult> AddUser([FromServices] RoleManager role, [FromServices] AEET.Code.UserManager user, UserMaster model)
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
                        roleId = (await role.GetRoleByName("Super Admin"))?.RoleID ?? Guid.Empty;
                        break;
                    case "AA Team":
                        roleId = (await role.GetRoleByName("Asset Administrator"))?.RoleID ?? Guid.Empty;
                        break;
                    case "TO":
                        roleId = (await role.GetRoleByName("Transaction Manager"))?.RoleID ?? Guid.Empty;
                        break;
                    default:
                        roleId = Guid.Empty;
                        break;
                }
                if (roleId == Guid.Empty)
                {
                    ModelState.AddModelError("role", "Invalid role selected.");
                    return View("Index", await user.GetUsers());
                }
                model.RoleID = roleId;

                // Set the creation properties
                model.UserID = Guid.NewGuid();  // Generate a new unique ID
                model.CreatedOn = DateTime.Now;
                model.CreatedBy = "system"; // Or the logged in admin's username

                // Optionally, you could hash the password here before saving

                // Add the new user to the database
                await user.AddUser(model);

                // Redirect back to the index view after success
                return RedirectToAction("Index");
            }
            // If model validation fails, redisplay the view with errors
            return View("Index", await user.GetUsers());
        }
    }
}
