using System;

namespace AEET.Models.DTOs
{
    /// <summary>
    /// DTO that contains data for Category, Vendor, Location, and Asset in a single request.
    /// </summary>
    public class CombinedDataDto
    {
        public CategoryDto Category { get; set; }
        public VendorDto Vendor { get; set; }
        public LocationDto Location { get; set; }
        public AssetDto Asset { get; set; }
    }

    public class CategoryDto
    {
        public string CategoryName { get; set; }
    }

    public class VendorDto
    {
        public string VendorName { get; set; }
        public string VendorType { get; set; }
    }

    public class LocationDto
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public int? LocationType { get; set; }
    }

    public class AssetDto
    {
        public string CompanyName { get; set; }
        public string AssetName { get; set; }
        public string AssetTag { get; set; }
        public string Model { get; set; }
        public string ModelNo { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? PurchasedDate { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? EOL { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public int? WarrantyPeriod { get; set; }
        public DateTime? WarrantyExpires { get; set; }
        public decimal? CurrentValue { get; set; }
        public bool? FullyDepreciated { get; set; }
        public DateTime? LastAudit { get; set; }
        public DateTime? NextAuditDate { get; set; }
        public string Notes { get; set; }
    }
}
