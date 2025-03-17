using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("Employee")]
    public class Employee
    {
        // Primary key: auto-incrementing integer
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        // EmployeeName NVARCHAR(100) UNIQUE (uniqueness can be enforced via Fluent API if needed)
        [Required]
        [StringLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        // Manager NVARCHAR(100)
        [StringLength(100)]
        public string? Manager { get; set; }

        // Department NVARCHAR(255)
        [StringLength(255)]
        public string? Department { get; set; }

        // Title NVARCHAR(255)
        [StringLength(255)]
        public string? Title { get; set; }

        // Phone NVARCHAR(50)
        [StringLength(50)]
        public string? Phone { get; set; }

        // Address NVARCHAR(255)
        [StringLength(255)]
        public string? Address { get; set; }

        // City NVARCHAR(100)
        [StringLength(100)]
        public string? City { get; set; }

        // State NVARCHAR(100)
        [StringLength(100)]
        public string? State { get; set; }

        // Country NVARCHAR(100)
        [StringLength(100)]
        public string? Country { get; set; }

        // ZipCode NVARCHAR(20)
        [StringLength(20)]
        public string? ZipCode { get; set; }

        // CreatedOn with default GETDATE() equivalent
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // CreatedBy NVARCHAR(255)
        [StringLength(255)]
        public string? CreatedBy { get; set; }

        // ModifiedOn with default GETDATE() equivalent
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        // ModifiedBy NVARCHAR(255)
        [StringLength(255)]
        public string? ModifiedBy { get; set; }
    }
}
