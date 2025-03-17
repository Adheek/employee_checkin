using AEET.Models;
using AEET.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AEET.Code
{
    public class EmployeeAssetMappingManager
    {
        private readonly ApplicationDbContext _context;

        public EmployeeAssetMappingManager(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new mapping record using the provided EmployeeId and AssetId.
        /// </summary>
        public async Task<EmployeeAssetMapping> CreateMappingAsync(int employeeId, string assetId, string createdBy)
        {
            var mapping = new EmployeeAssetMapping
            {
                EmployeeId = employeeId,
                AssetId = assetId,
                CreatedBy = createdBy,
                ModifiedBy = createdBy,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };
            _context.EmployeeAssetMappings.Add(mapping);
            await _context.SaveChangesAsync();
            return mapping;
        }

        /// <summary>
        /// Retrieves the list of mapped assets with related employee details.
        /// </summary>
        /// <returns>A list of mapped asset DTOs.</returns>
        public async Task<List<MappedAssetDto>> GetMappedAssetsAsync()
        {
            return await _context.EmployeeAssetMappings
                .Include(mapping => mapping.Employee)  // Ensure navigation property Employee exists
                .Include(mapping => mapping.Asset)       // Ensure navigation property Asset exists
                .Select(mapping => new MappedAssetDto
                {
                    AssetId = mapping.AssetId,
                    AssetName = mapping.Asset.AssetName,
                    EmployeeId = mapping.EmployeeId,
                    EmployeeName = mapping.Employee.EmployeeName,
                    Department = mapping.Employee.Department,
                    MappingDate = mapping.CreatedOn
                })
                .ToListAsync();
        }
    }
}
