using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("ScanDetails")]
    public class ScanDetail
    {
        [Key]
        public int ScanId { get; set; }

        public int EmployeeId { get; set; }
        public string AssetId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string TransactionType { get; set; }
        public string? MismatchedEmployeeId { get; set; }

        // Required navigation property to Employee
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}
