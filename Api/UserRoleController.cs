using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AEET.Api
{
    [Route("api/access")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        [HttpGet]
        [Route("users/list")]
        public async Task<dynamic> GetUserList([FromServices] AEET.Code.UserManager user)
        {
            // Query the UserMaster table and select only the columns you need
            // Return the result as JSON
            return await user.GetUserList();
        }
        [HttpGet]
        [Route("roles/list")]
        public async Task<dynamic> GetRoleList([FromServices] AEET.Code.RoleManager role)
        {
            // Query the UserMaster table and select only the columns you need
            // Return the result as JSON
            return await role.GetRoles();
        }
    }
}
