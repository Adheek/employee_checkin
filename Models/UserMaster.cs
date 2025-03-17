using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("UserMaster")]
    public class UserMaster
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        // EmailID: NVARCHAR(255) UNIQUE NOT NULL
        [Required]
        [StringLength(255)]
        public string EmailID { get; set; } = string.Empty;

        // PasswordHash: NVARCHAR(255) NOT NULL
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        // EmployeeName: NVARCHAR(255) NOT NULL
        [Required]
        [StringLength(255)]
        public string EmployeeName { get; set; } = string.Empty;

        // Username: NVARCHAR(255) NULL
        [StringLength(255)]
        public string? Username { get; set; }

        // EmployeeID: NVARCHAR(50) NULL
        [StringLength(50)]
        public string? EmployeeID { get; set; }

        // Location: NVARCHAR(255) NULL
        [StringLength(255)]
        public string? Location { get; set; }

        // RoleID: UNIQUEIDENTIFIER NOT NULL
        [Required]
        public Guid RoleID { get; set; }

        // IsActive: BIT DEFAULT 1
        public bool IsActive { get; set; } = true;

        // CreatedOn: DATETIME DEFAULT GETDATE()
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // CreatedBy: UNIQUEIDENTIFIER NULL
        public Guid? CreatedBy { get; set; }

        // ModifiedOn: DATETIME NULL
        public DateTime? ModifiedOn { get; set; }

        // ModifiedBy: UNIQUEIDENTIFIER NULL
        public Guid? ModifiedBy { get; set; }

        // Navigation property for RoleMaster
        [ForeignKey("RoleID")]
        public virtual RoleMaster? Role { get; set; }

        // Optionally, you can add navigation properties for CreatedBy and ModifiedBy if needed:
        // [ForeignKey("CreatedBy")]
        // public virtual UserMaster? CreatedByUser { get; set; }
        //
        // [ForeignKey("ModifiedBy")]
        // public virtual UserMaster? ModifiedByUser { get; set; }
    }
}
