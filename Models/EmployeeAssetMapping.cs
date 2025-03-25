using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("EmployeeAssetMapping")]
    public class EmployeeAssetMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MappedId { get; set; }

        // FK to Employee(EmployeeId)
        public int EmployeeId { get; set; }

        // FK to AssetMaster(AssetID)
        [StringLength(10)]
        public string AssetId { get; set; }

        // Audit fields
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
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        [ForeignKey(nameof(AssetId))]
        public virtual AssetMaster Asset { get; set; }
    }
}
