using AEET.Code;
using AEET.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AEET.Api
{
    [ApiController]
    [Route("api/asset")]
    public class AssetController : ControllerBase
    {
        private readonly AssetManager _assetManager;

        public AssetController(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAsset([FromBody] AssetMasterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Attempt to insert the new asset
            try
            {
                var createdAsset = await _assetManager.AddAsset(dto);
                return Ok(createdAsset);
            }
            catch (Exception ex)
            {
                // Return a generic 400 or 500 if something goes wrong
                return BadRequest(ex.Message);
            }
        }
    }
}
