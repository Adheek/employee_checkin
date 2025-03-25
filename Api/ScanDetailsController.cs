using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AEET.DTOs;
using AEET.Code;

namespace AEET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScanDetailsController : ControllerBase
    {
        private readonly ScanTransactionManager _transactionManager;

        // Single constructor accepting only ScanTransactionManager.
        public ScanDetailsController(ScanTransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        // POST api/ScanDetails/CheckIn
        [HttpPost("CheckIn")]
        public async Task<IActionResult> CheckIn([FromBody] ScanTransactionDto dto)
        {
            if (dto == null)
                return BadRequest("No scan data received.");

            try
            {
                await _transactionManager.ProcessCheckInAsync(dto);
                return Ok(new { success = true, message = "Check-in saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST api/ScanDetails/CheckOut
        [HttpPost("CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody] ScanTransactionDto dto)
        {
            if (dto == null)
                return BadRequest("No scan data received.");

            try
            {
                await _transactionManager.ProcessCheckOutAsync(dto);
                return Ok(new { success = true, message = "Check-out saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET api/ScanDetails/All
        [HttpGet("All")]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                // Retrieve logs using a function defined in ScanTransactionManager.
                var logs = await _transactionManager.GetAllScanLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
