using AEET.Models;
using AEET.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AEET.Code
{
    public class UserManager
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager _roleManager; // We'll inject RoleManager here

        public UserManager(ApplicationDbContext context, RoleManager roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Adds a user when you already have a fully constructed UserMaster entity.
        /// </summary>
        public async Task<UserMaster> AddUser(UserMaster model)
        {
            _context.UserMasters.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        /// <summary>
        /// Adds a user from a DTO, including mapping the role string to a RoleID.
        /// </summary>
        public async Task<UserMaster> AddUser(UserMasterDto model)
        {
            // 1. Map the 'Role' string (e.g., "SA", "AA Team", "TO") to the actual RoleID
            string roleDropdown = model.Role;
            Guid roleId;

            switch (roleDropdown)
            {
                case "SA":
                    roleId = (await _roleManager.GetRoleByName("Super Admin"))?.RoleID ?? Guid.Empty;
                    break;
                case "AA Team":
                    roleId = (await _roleManager.GetRoleByName("Asset Administrator"))?.RoleID ?? Guid.Empty;
                    break;
                case "TO":
                    roleId = (await _roleManager.GetRoleByName("Transaction Manager"))?.RoleID ?? Guid.Empty;
                    break;
                default:
                    roleId = Guid.Empty;
                    break;
            }

            if (roleId == Guid.Empty)
            {
                // Throw an exception so the caller (controller) can return 400 or handle it as needed
                throw new ArgumentException("Invalid role selected.");
            }

            // 2. Create a new UserMaster entity from the DTO data
            var newUser = new UserMaster
            {
                UserID = Guid.NewGuid(),
                Username = model.Username,
                PasswordHash = model.PasswordHash,
                EmployeeName = model.EmployeeName,
                EmployeeID = model.EmployeeID,
                EmailID = model.EmailID,
                RoleID = roleId,
                Location = model.Location,
                CreatedOn = DateTime.Now,
                CreatedBy = null
            };

            // 3. Save the user
            _context.UserMasters.Add(newUser);
            await _context.SaveChangesAsync();

            // 4. Return the newly created user
            return newUser;
        }

        public async Task<IEnumerable<UserMaster>> GetUsers()
        {
            // Eager-load the Role entity so you can display RoleName if needed
            return await _context.UserMasters
                .Include(u => u.Role)
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetUserList()
        {
            // Eager load the Role so you can reference RoleName
            return await _context.UserMasters
                .Include(u => u.Role)
                .Select(u => new
                {
                    Username = u.Username,
                    EmployeeName = u.EmployeeName,
                    Email = u.EmailID,
                    RoleName = u.Role.RoleName,
                    Location = u.Location
                })
                .ToListAsync();
        }
    }
}
