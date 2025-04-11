using AEET.Code;
using AEET.Models.DTOs;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AEET.Models;
using ExcelDataReader.Exceptions;

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

        /// <summary>
        /// Upload and process an Excel file containing template records.
        /// </summary>
        /// <param name="model">The upload model containing the file.</param>
        /// <returns>A JSON response indicating success or failure.</returns>
        [HttpPost("BulkUploadAssetDetails")]
        public async Task<IActionResult> BulkUploadAssetDetails([FromForm] BulkUploadDTO model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest(new { message = "Please select an Excel file to upload." });
            }

            var records = new List<BulkUploadTemplateRecordModel>();

            // Copy the uploaded file to a MemoryStream
            using (var stream = new MemoryStream())
            {
                await model.File.CopyToAsync(stream);
                stream.Position = 0;

                // Register the code pages provider for ExcelDataReader
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                try 
                {
                    // Create the reader for the Excel file
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Convert the data to a DataSet, where each sheet becomes a DataTable
                        var result = reader.AsDataSet();
                        if (result.Tables.Count == 0)
                        {
                            return BadRequest(new { message = "The Excel file is empty or invalid." });
                        }

                        var table = result.Tables[0];
                        int rowCount = table.Rows.Count;

                        // Assuming the first row contains headers, data starts from row index 1
                        for (int rowIndex = 1; rowIndex < rowCount; rowIndex++)
                        {
                            var row = table.Rows[rowIndex];

                            // Skip rows with missing or invalid data

                            var assetName = row[1]?.ToString();
                            var assetTag = row[2]?.ToString();
                            var serialNumber = row[3]?.ToString();

                            if (string.IsNullOrWhiteSpace(assetName) || string.IsNullOrWhiteSpace(assetTag) || string.IsNullOrWhiteSpace(serialNumber))
                            {
                                continue;
                            }

                            records.Add(new BulkUploadTemplateRecordModel
                            {
                                // Adjust column indices based on your Excel template (0-based index)
                                Company = row[0]?.ToString(),
                                Asset_Name = assetName,
                                Asset_Tag = assetTag,
                                Serial_Number = serialNumber,
                                Model_Name = row[4]?.ToString(),
                                Model_Number = row[5]?.ToString(),
                                Category = row[6]?.ToString(),
                                Location = row[7]?.ToString(),
                                Manufacturer = row[8]?.ToString(),
                                Supplier = row[9]?.ToString(),
                                Purchase_Date = row[10]?.ToString(),
                                Purchase_Cost = row[11]?.ToString(),
                                Order_Number = row[12]?.ToString(),
                                Warranty = row[13]?.ToString(),
                                Status = row[14]?.ToString()
                            });
                        }
                    }
                }
                catch (HeaderException ex)
                {
                    return BadRequest(new { message = "Excel File Header reading error" });
                }
                catch (ExcelReaderException ex)
                {
                    return BadRequest(new { message = "Error Reading uploaded excel file." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Error Reading uploaded excel file." });
                }
            }

            // Insert the parsed records in bulk if any valid records were found
            if (records.Any())
            {
                bool success = true;
                List<FailedAssetUploadModel> failedrecordsList = new List<FailedAssetUploadModel>();
                int numberOfAssetsUploaded = _fullDataManager.BulkUploadAssets(records, out success, out failedrecordsList);
                if (!success && numberOfAssetsUploaded < 1)
                    return Ok(new { message = $"Request completed without completing bulk asset upload operation." });
                else
                    return Ok(new { message = $"{numberOfAssetsUploaded} record(s) processed successfully.", failedrecordsList });
            }
            else
            {
                return BadRequest(new { message = "No valid records found in the Excel file." });
            }
        }
    }
}
