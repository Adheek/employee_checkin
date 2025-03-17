using AEET.Models; // Ensure this namespace contains ApplicationDbContext and Employee model.
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AEET.Code
{
    public class EmployeeManager
    {
        private readonly ApplicationDbContext _context;

        public EmployeeManager(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Searches employees by the beginning of their EmployeeId (converted to string).
        /// </summary>
        /// <param name="term">Partial string to match on EmployeeId.</param>
        /// <returns>List of matching Employee objects.</returns>
        public async Task<List<Employee>> SearchEmployeeByIdAsync(string term)
        {
            return await _context.Employees
                .Where(e => e.EmployeeId.ToString().StartsWith(term))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single employee by their EmployeeId.
        /// </summary>
        /// <param name="employeeId">The Employee Id to search for.</param>
        /// <returns>The matching Employee object if found, otherwise null.</returns>
        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }
    }
}
