using Azure;
using Microsoft.Extensions.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AEET.Models
{
    public class BulkUploadTemplateRecordModel
    {
        public string? Company { get; set; }
        public string Asset_Name { get; set; }
        public string Asset_Tag { get; set; }
        public string Serial_Number { get; set; }
        public string? Model_Name { get; set; }
        public string? Model_Number { get; set; }
        public string? Category { get; set; }
        public string? Location { get; set; }
        public string? Manufacturer { get; set; }
        public string? Supplier { get; set; } 
        public string? Purchase_Date { get; set; }
        public string? Purchase_Cost { get; set; }
        public string? Order_Number { get; set; }
        public string? Warranty { get; set; }
        public string? Months { get; set; }
        public string? Status { get; set; }
    }
}
