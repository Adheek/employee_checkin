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
        private readonly RoleManager _roleManager;

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
        /// Throws an ArgumentException if the role or EmployeeID is invalid.
        /// </summary>
        public async Task<UserMaster> AddUser(UserMasterDto model)
        {
            if (string.IsNullOrWhiteSpace(model.EmployeeID))
            {
                throw new ArgumentException("Employee ID cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(model.Username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            // 1. Map the 'Role' string (e.g., "SA", "AA Team", "TO") to the actual RoleID
            Guid roleId = await GetRoleIdFromDropdownValue(model.Role);
            if (roleId == Guid.Empty)
            {
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

        /// <summary>
        /// Updates an existing user based on the given DTO.
        /// Throws an ArgumentException if the user is not found or invalid data is supplied.
        /// </summary>
        public async Task<UserMaster> UpdateUser(UserMasterDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            // Find the existing user by username (or however you identify them)
            var existingUser = await _context.UserMasters
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (string.IsNullOrWhiteSpace(model.EmployeeID))
            {
                throw new ArgumentException("Employee ID cannot be empty.");
            }

            // Map role the same way as in AddUser
            Guid roleId = await GetRoleIdFromDropdownValue(model.Role);
            if (roleId == Guid.Empty)
            {
                throw new ArgumentException("Invalid role selected.");
            }

            // Update fields
            existingUser.PasswordHash  = model.PasswordHash;
            existingUser.EmployeeName  = model.EmployeeName;
            existingUser.EmployeeID    = model.EmployeeID;
            existingUser.EmailID       = model.EmailID;
            existingUser.RoleID        = roleId;
            existingUser.Location      = model.Location;
            existingUser.ModifiedOn    = DateTime.Now;
            existingUser.ModifiedBy    = null;

            // Save changes
            await _context.SaveChangesAsync();
            return existingUser;
        }

        /// <summary>
        /// Retrieves all users, including Role entity.
        /// </summary>
        public async Task<IEnumerable<UserMaster>> GetUsers()
        {
            return await _context.UserMasters
                .Include(u => u.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Returns a user list with selected fields, including RoleName.
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetUserList()
        {
            return await _context.UserMasters
                .Include(u => u.Role)
                .Select(u => new
                {
                    Username     = u.Username,
                    EmployeeName = u.EmployeeName,
                    Email        = u.EmailID,
                    RoleName     = u.Role.RoleName,
                    Location     = u.Location,
                    // If you want EmployeeID in the list:
                    EmployeeID   = u.EmployeeID,
                    // If you want to return the password hash for editing (NOT recommended in production):
                    PasswordHash = u.PasswordHash
                })
                .ToListAsync();
        }

        /// <summary>
        /// Helper method to map the dropdown value (SA, AA Team, TO) to the actual RoleID.
        /// Returns Guid.Empty if no matching role is found.
        /// </summary>
        private async Task<Guid> GetRoleIdFromDropdownValue(string roleDropdown)
        {
            switch (roleDropdown)
            {
                case "SA":
                    return (await _roleManager.GetRoleByName("Super Admin"))?.RoleID ?? Guid.Empty;
                case "AA Team":
                    return (await _roleManager.GetRoleByName("Asset Administrator"))?.RoleID ?? Guid.Empty;
                case "TO":
                    return (await _roleManager.GetRoleByName("Transaction Manager"))?.RoleID ?? Guid.Empty;
                default:
                    return Guid.Empty;
            }
        }
    }
}
