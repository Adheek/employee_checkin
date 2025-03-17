using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using AEET.Code;    // Contains EmployeeManager
using AEET.Models;  // Contains Employee model

namespace AEET.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeManager _employeeManager;

        public EmployeeController(EmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        [HttpGet("searchEmployees")]
        public async Task<IActionResult> SearchEmployees([FromQuery] string term)
        {
            // Call the updated manager method which searches by EmployeeId (converted to string)
            var employees = await _employeeManager.SearchEmployeeByIdAsync(term);

            // Transform the result for the autocomplete widget using EmployeeId (converted to string)
            var result = employees.Select(e => new 
            {
                label = e.EmployeeId.ToString(),
                value = e.EmployeeId.ToString(),
                employeeName = e.EmployeeName
            });

            return Ok(result);
        }
    }
}
