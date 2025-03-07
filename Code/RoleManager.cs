using AEET.Models;

namespace AEET.Code
{
    public class RoleManager
    {
        ApplicationDbContext _context;
        public RoleManager(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RoleMaster>> GetRoles()
        {
            return _context.RoleMasters;
        }
        public async Task<RoleMaster> GetRoleByName(string role_name)
        {
            return _context.RoleMasters.FirstOrDefault(r => r.RoleName == role_name);
        }
    }
}
