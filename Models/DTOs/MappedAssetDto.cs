using Microsoft.EntityFrameworkCore;
using System;

namespace AEET.Models.DTOs
{
    [Keyless]
    public class MappedAssetDto
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime MappingDate { get; set; }
    }
}
