using AEET.Code;
using AEET.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AEET.Models;

namespace AEET.Api
{
    [ApiController]
    [Route("api/fullData")]
    public class FullDataController : ControllerBase
    {
        private readonly FullDataManager _fullDataManager;

        public FullDataController(FullDataManager fullDataManager)
        {
            _fullDataManager = fullDataManager;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddFullData([FromBody] CombinedDataDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdAsset = await _fullDataManager.AddFullDataAsync(dto);
                return Ok(createdAsset);
            }
            catch (Exception ex)
            {
                // Log or handle error as needed
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("getAll")]
        public async Task<ActionResult<List<AssetMaster>>> GetAllAssets()
        {
            var assets = await _fullDataManager.GetAllAssetsAsync();
            return Ok(assets);
        }


        [HttpGet("searchAssets")]
        public async Task<IActionResult> SearchAssets([FromQuery] string term)
        {
            var assets = await _fullDataManager.SearchAssetsAsync(term);

            // Return JSON with label, value, and assetName
            var result = assets.Select(a => new
            {
                label = a.AssetID.ToString(),       // What appears in the dropdown
                value = a.AssetID.ToString(),       // The actual value once selected
                assetName = a.AssetName            // We'll use this to fill the "Asset Name" field
            });

            return Ok(result);
        }




    }
}
