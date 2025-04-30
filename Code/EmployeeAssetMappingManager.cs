// EmployeeAssetMappingManager.cs
using AEET.Models;
using AEET.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        /// Creates a new mapping record.
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
        /// Retrieves all mappings.
        /// </summary>
        public async Task<List<MappedAssetDto>> GetMappedAssetsAsync()
        {
            return await _context.EmployeeAssetMappings
                .Include(m => m.Employee)
                .Include(m => m.Asset)
                .Select(m => new MappedAssetDto
                {
                    AssetId = m.AssetId,
                    AssetName = m.Asset.AssetName,
                    EmployeeId = m.EmployeeId,
                    EmployeeName = m.Employee.EmployeeName,
                    Department = m.Employee.Department,
                    MappingDate = m.CreatedOn
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single mapping by AssetId.
        /// </summary>
        public async Task<MappedAssetDto> GetMappingByAssetIdAsync(string assetId)
        {
            // Normalize to lower-case for translation in EF Core
            var lookup = assetId?.ToLower() ?? string.Empty;

            return await _context.EmployeeAssetMappings
                .Include(m => m.Employee)
                .Include(m => m.Asset)
                .Where(m => m.AssetId.ToLower() == lookup)
                .Select(m => new MappedAssetDto
                {
                    AssetId = m.AssetId,
                    AssetName = m.Asset.AssetName,
                    EmployeeId = m.EmployeeId,
                    EmployeeName = m.Employee.EmployeeName,
                    Department = m.Employee.Department,
                    MappingDate = m.CreatedOn
                })
                .FirstOrDefaultAsync();
        }
    }
}
