using AEET.Models;
using System.Collections;

namespace AEET.Code
{
    public class UserManager
    {
        ApplicationDbContext _context;
        public UserManager(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UserMaster> AddUser(UserMaster model)
        {
            _context.UserMasters.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }
        public async Task<IEnumerable<UserMaster>> GetUsers()
        {
            return _context.UserMasters;
        }
        public async Task<IEnumerable<dynamic>> GetUserList()
        {
            return _context.UserMasters
                .Select(u => new
                {
                    u.Username,
                    u.EmployeeName,
                    Email = u.EmailID,
                    RoleName = u.Role.RoleName, // using the navigation property for Role
                    u.Location
                })
                .ToList();
        }
    }
}
