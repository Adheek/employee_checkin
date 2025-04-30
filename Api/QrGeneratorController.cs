using System.Threading.Tasks;
using AEET.Code;
using Microsoft.AspNetCore.Mvc;

namespace AEET.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrGeneratorController : ControllerBase
    {
        private readonly QrGeneratorManager _manager;

        public QrGeneratorController(QrGeneratorManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// GET api/qrGenerator?name=foo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Query parameter 'name' is required.");

            var assets = await _manager.SearchByAssetNameAsync(name);
            return Ok(assets);
        }
    }
}
