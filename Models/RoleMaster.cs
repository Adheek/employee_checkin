using System;
using System.ComponentModel.DataAnnotations;

namespace AEET.Models
{
    public class RoleMaster
    {
        [Key]
        public Guid RoleID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? RoleName { get; set; }
        
        // Audit properties - these are optional depending on your needs.
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        [StringLength(255)]
        public string? ModifiedBy { get; set; }
        
        public DateTime? ModifiedOn { get; set; }
    }
}
