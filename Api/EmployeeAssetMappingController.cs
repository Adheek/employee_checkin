using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AEET.Code;             // For EmployeeAssetMappingManager
using AEET.Models.DTOs;      // For MapAssetDto
using System.Collections.Generic;




namespace AEET.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeAssetMappingController : ControllerBase
    {
        private readonly EmployeeAssetMappingManager _mappingManager;

        public EmployeeAssetMappingController(EmployeeAssetMappingManager mappingManager)
        {
            _mappingManager = mappingManager;
        }

        [HttpPost("mapAsset")]
        public async Task<IActionResult> MapAsset([FromBody] MapAssetDto dto)
        {
            try
            {
                // Now uses dto.EmployeeId which should be of type int (updated to match the new schema)
                var mapping = await _mappingManager.CreateMappingAsync(dto.EmployeeId, dto.AssetId, dto.CreatedBy);
                return Ok(mapping);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        /// <summary>
        /// Retrieves all mapped assets.
        /// </summary>
        /// <returns>List of mapped asset DTOs.</returns>
        [HttpGet("mappedAssets")]
        public async Task<IActionResult> GetMappedAssets()
        {
            List<MappedAssetDto> mappedAssets = await _mappingManager.GetMappedAssetsAsync();
            return Ok(mappedAssets);
        }
    }
}
