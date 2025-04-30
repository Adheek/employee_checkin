// EmployeeAssetMappingController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AEET.Code;            // EmployeeAssetMappingManager
using AEET.Models.DTOs;     // MapAssetDto, MappedAssetDto

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

        /// <summary>
        /// Create a new asset–employee mapping.
        /// POST /api/EmployeeAssetMapping/mapAsset
        /// </summary>
        [HttpPost("mapAsset")]
        public async Task<IActionResult> MapAsset([FromBody] MapAssetDto dto)
        {
            if (dto == null)
                return BadRequest(new { error = "No mapping data received." });

            try
            {
                var mapping = await _mappingManager.CreateMappingAsync(
                    dto.EmployeeId,
                    dto.AssetId,
                    dto.CreatedBy
                );
                return Ok(mapping);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Returns all mappings.
        /// GET /api/EmployeeAssetMapping/mappedAssets
        /// </summary>
        [HttpGet("mappedAssets")]
        public async Task<IActionResult> GetMappedAssets()
        {
            var mappedAssets = await _mappingManager.GetMappedAssetsAsync();
            return Ok(mappedAssets);
        }

        /// <summary>
        /// Legacy lookup by asset ID:
        /// GET /api/EmployeeAssetMapping/{assetId}
        /// </summary>
        [HttpGet("{assetId}")]
        public async Task<IActionResult> GetByAssetId_Legacy(string assetId)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                return BadRequest(new { error = "AssetId is required." });

            try
            {
                var map = await _mappingManager.GetMappingByAssetIdAsync(assetId);
                if (map == null)
                    return NotFound(new { error = $"No mapping found for '{assetId}'." });

                return Ok(new
                {
                    assetId = map.AssetId,
                    assetName = map.AssetName,
                    employeeId = map.EmployeeId,
                    employeeName = map.EmployeeName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// New explicit route for clarity:
        /// GET /api/EmployeeAssetMapping/asset/{assetId}
        /// </summary>
        [HttpGet("asset/{assetId}")]
        public Task<IActionResult> GetByAssetId(string assetId)
            => GetByAssetId_Legacy(assetId);
    }
}
