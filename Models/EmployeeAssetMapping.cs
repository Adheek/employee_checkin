using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("EmployeeAssetMapping")]
    public class EmployeeAssetMapping
    {
        [Key]
        public int MappedId { get; set; }
        
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        [StringLength(10)]
        public string AssetId { get; set; }
        
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        
        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; } = string.Empty;
        
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        
        [Required]
        [StringLength(255)]
        public string ModifiedBy { get; set; } = string.Empty;
        
        public int? UnmappedID { get; set; }

        // Navigation properties
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("AssetId")]
        public virtual AssetMaster Asset { get; set; }
    }
}
