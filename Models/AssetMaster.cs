using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("AssetMaster")]
    public class AssetMaster
    {
        [Key]
        [StringLength(10)]
        public string? AssetID { get; set; }  // Do not set this in code; let SQL generate it.

        [StringLength(255)]
        public string? CompanyName { get; set; }

        [StringLength(255)]
        public string? AssetName { get; set; }

        [StringLength(50)]
        public string? AssetTag { get; set; }

        [StringLength(255)]
        public string? Model { get; set; }

        [StringLength(50)]
        public string? ModelNo { get; set; }

        // Foreign Keys (nullable GUIDs)
        public Guid? CategoryID { get; set; }
        public Guid? ManufacturerID { get; set; }
        public Guid? SupplierID { get; set; }
        public Guid? DefaultLocationID { get; set; }

        [StringLength(100)]
        public string? SerialNumber { get; set; } = string.Empty;

        public DateTime? PurchasedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cost { get; set; }

        public DateTime? EOL { get; set; }

        [StringLength(50)]
        public string? OrderNumber { get; set; }

        [StringLength(100)]
        public string? Status { get; set; }

        public int? WarrantyPeriod { get; set; }
        public DateTime? WarrantyExpires { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentValue { get; set; }

        public bool FullyDepreciated { get; set; } = false;

        public DateTime? LastAudit { get; set; }
        public DateTime? NextAuditDate { get; set; }

        public string? Notes { get; set; }

        // Audit Fields
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? ModifiedBy { get; set; }

        public bool Deleted { get; set; } = false;

        [ForeignKey("CategoryID")]
        public CategoryMaster? Category { get; set; }

        [ForeignKey("DefaultLocationID")]
        public LocationMaster? DefaultLocation { get; set; }
    }
}
