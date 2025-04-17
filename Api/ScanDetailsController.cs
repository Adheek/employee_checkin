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

        public ScanDetailsController(ScanTransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

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

        [HttpGet("All")]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _transactionManager.GetAllScanLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }



        // POST api/ScanDetails/Process
        [HttpPost("Process")]
        public async Task<IActionResult> Process([FromBody] ScanTransactionDto dto)
        {
            if (dto == null)
                return BadRequest(new { success = false, message = "No scan data received." });

            try
            {
                // --- ADD THIS LINE ---
                Console.WriteLine($"[ScanDetailsController] Process called at {DateTime.UtcNow:O} " +
                                  $"– AssetId={dto.AssetId}, EmployeeId={dto.EmployeeId}, EmployeeName={dto.EmployeeName}");

                // This will automatically decide CheckIn vs CheckOut based on your 17‑hour rule
                await _transactionManager.ProcessScanAsync(dto);

                Console.WriteLine($"[ScanDetailsController] Process completed successfully for AssetId={dto.AssetId}");

                return Ok(new { success = true, message = "Scan processed successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ScanDetailsController] Process error: {ex}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }





        [HttpPost("GetAllAssetTransaction")]
        public async Task<IActionResult> GetAllAssetTransaction([FromBody] ScanTransactionDto dto)
        {
            if (dto == null)
                return BadRequest("No scan data received.");

            try
            {
                var logs = await _transactionManager.GetAllTransaction(dto);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
