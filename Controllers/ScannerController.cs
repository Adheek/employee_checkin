using Microsoft.AspNetCore.Mvc;

namespace AEET.Controllers
{
    // Simple model to hold scanned data
    public class ScanDataModel
    {
        public string? ScannedData { get; set; }
    }

    [Route("Scanner")]
    public class ScannerController : Controller
    {
        // Parameterless constructor only – no dependency injection here.
        public ScannerController()
        {
        }

        // GET: /Scanner/Index
        // GET: /Scanner/Index
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // First check for query parameter
            var scanDataFromQuery = Request.Query["scanned"].ToString();

            // Then check TempData
            var scanDataFromTemp = TempData["ScannedData"] as string;

            // Use query parameter first, then fall back to TempData
            ViewBag.ScannedData = !string.IsNullOrEmpty(scanDataFromQuery)
                ? scanDataFromQuery
                : scanDataFromTemp;

            return View("~/Views/Scanner/Index.cshtml");
        }

        // POST: /Scanner/ProcessScan
        [HttpPost("ProcessScan")]
        public IActionResult ProcessScan([FromBody] ScanDataModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.ScannedData))
            {
                return BadRequest("Invalid scan data received");
            }

            // Store the scanned data in TempData for later display by the Index view.
            TempData["ScannedData"] = model.ScannedData;
            Console.WriteLine($"Received scan data: {model.ScannedData}");

            return Ok(new { success = true, message = "Scan data received successfully" });
        }
    }
}
