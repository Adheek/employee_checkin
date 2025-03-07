using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("UserMaster")]
    public class UserMaster
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid(); // Primary key, generated automatically

        [Required]
        [StringLength(255)]
        public string? Username { get; set; }  // For login; this could be the same as EmailID if desired

        [Required]
        [StringLength(255)]
        public string? PasswordHash { get; set; }  // Ideally store a hashed password

        [Required]
        [StringLength(255)]
        public string? EmployeeName { get; set; }  // Replaces FullName

        [Required]
        [StringLength(100)]
        public string? EmployeeID { get; set; }    // New column for employee id

        [Required]
        [StringLength(255)]
        public string? EmailID { get; set; }

        [Required]
        public Guid RoleID { get; set; }          // Foreign key to RoleMaster

        [Required]
        [StringLength(255)]
        public string? Location { get; set; }      // New column for Location

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [StringLength(255)]
        public string? ModifiedBy { get; set; }

        // Navigation property
        [ForeignKey("RoleID")]
        public virtual RoleMaster? Role { get; set; }
    }
}
