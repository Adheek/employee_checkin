using AEET.Models;
using AEET.Models.DTOs;  // Ensure your CombinedDataDto and related DTOs are here.
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;    // <-- Added for List<T>
using System.Linq;                   // <-- Added for .Where(...)
using System.Threading.Tasks;

namespace AEET.Code
{
    public class FullDataManager
    {
        private readonly ApplicationDbContext _context;

        public FullDataManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AssetMaster> AddFullDataAsync(CombinedDataDto dto)
        {
            // Start a transaction to ensure all inserts succeed or fail as one.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Handle Category
                var category = await _context.CategoryMasters
                    .FirstOrDefaultAsync(c => c.CategoryName == dto.Category.CategoryName);

                if (category == null)
                {
                    category = new CategoryMaster
                    {
                        CategoryName = dto.Category.CategoryName,
                        CreatedOn = DateTime.Now,
                        CreatedBy = "system",
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = "system"
                    };
                    _context.CategoryMasters.Add(category);
                    await _context.SaveChangesAsync();
                }

                // 2. Handle Vendor (for both Manufacturer and Supplier)
                var vendor = await _context.VendorMasters
                    .FirstOrDefaultAsync(v => v.Name == dto.Vendor.VendorName && v.VendorType == dto.Vendor.VendorType);

                if (vendor == null)
                {
                    vendor = new VendorMaster
                    {
                        Name = dto.Vendor.VendorName,
                        VendorType = dto.Vendor.VendorType,
                        CreatedOn = DateTime.Now,
                        CreatedBy = "system",
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = "system"
                    };
                    _context.VendorMasters.Add(vendor);
                    await _context.SaveChangesAsync();
                }

                // 3. Handle Location
                var location = await _context.LocationMasters
                    .FirstOrDefaultAsync(l =>
                        l.Country == dto.Location.Country &&
                        l.State == dto.Location.State &&
                        l.City == dto.Location.City &&
                        l.Address == dto.Location.Address &&
                        l.ZipCode == dto.Location.ZipCode &&
                        l.LocationType == (dto.Location.LocationType ?? 0));

                if (location == null)
                {
                    location = new LocationMaster
                    {
                        Country = dto.Location.Country,
                        State = dto.Location.State,
                        City = dto.Location.City,
                        Address = dto.Location.Address,
                        ZipCode = dto.Location.ZipCode,
                        LocationType = dto.Location.LocationType ?? 0,
                        CreatedOn = DateTime.Now,
                        CreatedBy = "system",
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = "system"
                    };
                    _context.LocationMasters.Add(location);
                    await _context.SaveChangesAsync();
                }

                // 4. Create the Asset
                var asset = new AssetMaster
                {
                    CompanyName = dto.Asset.CompanyName,
                    AssetName = dto.Asset.AssetName,
                    AssetTag = dto.Asset.AssetTag,
                    Model = dto.Asset.Model,
                    ModelNo = dto.Asset.ModelNo,
                    SerialNumber = dto.Asset.SerialNumber,
                    PurchasedDate = dto.Asset.PurchasedDate,
                    Cost = dto.Asset.Cost,
                    EOL = dto.Asset.EOL,
                    OrderNumber = dto.Asset.OrderNumber,
                    Status = dto.Asset.Status,
                    WarrantyPeriod = dto.Asset.WarrantyPeriod,
                    WarrantyExpires = dto.Asset.WarrantyExpires,
                    CurrentValue = dto.Asset.CurrentValue,
                    FullyDepreciated = dto.Asset.FullyDepreciated ?? false,
                    LastAudit = dto.Asset.LastAudit,
                    NextAuditDate = dto.Asset.NextAuditDate,
                    Notes = dto.Asset.Notes,
                    CreatedOn = DateTime.Now,
                    CreatedBy = "system",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "system",

                    // Link the Asset to the related records.
                    CategoryID = category.CategoryID,
                    ManufacturerID = vendor.VendorID,
                    SupplierID = vendor.VendorID,  // If the same vendor is used for both manufacturer and supplier.
                    DefaultLocationID = location.LocationID
                };

                _context.AssetMasters.Add(asset);
                await _context.SaveChangesAsync();

                // Commit the transaction.
                await transaction.CommitAsync();

                return asset;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // NEW METHOD: Fetch all assets (excluding deleted), including Category and Location if navigation properties exist.
        public async Task<List<AssetMaster>> GetAllAssetsAsync()
        {
            return await _context.AssetMasters
                .Where(a => !a.Deleted)           // Only get assets that are not marked deleted
                .Include(a => a.Category)         // If you have a navigation property for Category
                .Include(a => a.DefaultLocation)  // If you have a navigation property for Location
                .ToListAsync();
        }

        public async Task<List<AssetMaster>> SearchAssetsAsync(string term)
        {
            // Searches the AssetID as a string for anything that starts with 'term'
            return await _context.AssetMasters
                .Where(a => a.AssetID.ToString().StartsWith(term))
                .ToListAsync();
        }


    }
}
