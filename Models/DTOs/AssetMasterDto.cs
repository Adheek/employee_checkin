using System;

namespace AEET.Models.DTOs
{
    public class AssetMasterDto
    {
        // Optional: If you let the database auto-generate the AssetID, you can leave this blank.
        public string AssetId { get; set; }
        
        // Basic Information
        public string CompanyName { get; set; }
        public string AssetName { get; set; }
        public string AssetTag { get; set; }
        public string Model { get; set; }
        public string ModelNo { get; set; }
        public string SerialNumber { get; set; }

        // Foreign Key References (as strings; will be parsed to GUIDs)
        public string CategoryID { get; set; }
        public string ManufacturerID { get; set; }
        public string SupplierID { get; set; }
        public string DefaultLocationID { get; set; }

        // Purchase & Warranty Information
        public DateTime? PurchasedDate { get; set; }
        public decimal? Cost { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public int? WarrantyPeriod { get; set; }
        public DateTime? WarrantyExpires { get; set; }
        public DateTime? EOL { get; set; }
        public decimal? CurrentValue { get; set; }

        // Audit or Additional Information (optional input from client)
        public DateTime? LastAudit { get; set; }
        public DateTime? NextAuditDate { get; set; }
        public string Notes { get; set; }
    }
}
