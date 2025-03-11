using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AEET.Code;
using AEET.Models;
using AEET.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace AEET.Api
{
    [Route("api/access")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        [HttpGet("users/list")]
        public async Task<IActionResult> GetUserList([FromServices] UserManager user)
        {
            var result = await user.GetUserList();
            return Ok(result);
        }

        [HttpGet("roles/list")]
        public async Task<IActionResult> GetRoleList([FromServices] RoleManager role)
        {
            var result = await role.GetRoles();
            return Ok(result);
        }

        [HttpPost("users/add")]
        public async Task<IActionResult> AddUser(
    [FromServices] UserManager user,
    [FromBody] UserMasterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var addedUser = await user.AddUser(model);
                return Ok(addedUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
