using Microsoft.AspNetCore.Mvc;
using AeeT_MVC.Models; 

namespace AeeT_MVC.Controllers
{
    // Simple model to hold scanned data
    public class ScanDataModel
    {
        public string? ScannedData { get; set; }
    }

    // Route all actions to /Scanner/...
    [Route("Scanner")]
    public class ScannerController : Controller
    {
        // GET: /Scanner/Index
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Retrieve scanned data from TempData, if any
            var scanData = TempData["ScannedData"] as string;
            ViewBag.ScannedData = scanData;

            // Return the Scanner/Index.cshtml view
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

            // Store the scanned data in TempData so Index can display it
            TempData["ScannedData"] = model.ScannedData;

            // Log or process the scanned data as needed
            Console.WriteLine($"Received scan data: {model.ScannedData}");

            // Return OK so the client knows it succeeded
            return Ok(new { success = true, message = "Scan data received successfully" });
        }
    }
}
