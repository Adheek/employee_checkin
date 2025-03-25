using System;

namespace AEET.DTOs
{
    /// <summary>
    /// DTO representing a scan transaction.
    /// </summary>
    public class ScanTransactionDto
    {
        public string AssetId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string? MismatchedEmployeeId { get; set; }
    }
}
