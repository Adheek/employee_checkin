using AEET.Models;
using AEET.Models.DTOs;  // Ensure your CombinedDataDto and related DTOs are here.
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

                    // 4. Create the Asset.
                    // IMPORTANT: Do not assign any value to AssetID; let SQL Server generate it via the default constraint.
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
                        AssetEntryProcess = "Manual",
                        CreatedOn = DateTime.Now,
                        CreatedBy = "system",
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = "system",
                        // Link to related records.
                        CategoryID = category.CategoryID,
                        ManufacturerID = vendor.VendorID,
                        SupplierID = vendor.VendorID,  // Using the same vendor for both if applicable.
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

        // Fetch all assets (excluding deleted), including related Category and Location.
        public async Task<List<AssetMaster>> GetAllAssetsAsync()
        {
            return await _context.AssetMasters
                .Where(a => !a.Deleted)
                .Include(a => a.Category)
                .Include(a => a.DefaultLocation)
                .ToListAsync();
        }

        public async Task<List<AssetMaster>> SearchAssetsAsync(string term)
        {
            // Searches AssetID for values that start with the specified term.
            return await _context.AssetMasters
                .Where(a => a.AssetID != null && a.AssetID.StartsWith(term))
                .ToListAsync();
        }

        public int BulkUploadAssets(List<BulkUploadTemplateRecordModel> assetsList, out bool success, out List<FailedAssetUploadModel> failedRecords)
        {
            failedRecords = new List<FailedAssetUploadModel>();
            try
            {
                List<AssetMaster> assetMasterList = new List<AssetMaster>();

                foreach (var asset in assetsList)
                {
                    try
                    {
                        var isAssetinDB = _context.AssetMasters.Any(c => c.AssetName == asset.Asset_Name && c.AssetTag == asset.Asset_Tag && c.SerialNumber == asset.Serial_Number);
                        if (!isAssetinDB)
                        {
                            // Attempt to find related records and convert fields
                            var category = _context.CategoryMasters.FirstOrDefault(c => c.CategoryName == asset.Category);
                            if (category == null && !String.IsNullOrEmpty(asset.Category))
                            {
                                category = new CategoryMaster
                                {
                                    CategoryName = asset.Category,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = "system",
                                    ModifiedOn = DateTime.Now,
                                    ModifiedBy = "system"
                                };
                                _context.CategoryMasters.Add(category);
                                _context.SaveChangesAsync();
                            }

                            var vendor = _context.VendorMasters.FirstOrDefault(v => v.Name == asset.Manufacturer && v.VendorType == asset.Supplier);
                            if (vendor == null && !String.IsNullOrEmpty(asset.Manufacturer) && !String.IsNullOrEmpty(asset.Supplier))
                            {
                                vendor = new VendorMaster
                                {
                                    Name = asset.Manufacturer,
                                    VendorType = asset.Supplier,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = "system",
                                    ModifiedOn = DateTime.Now,
                                    ModifiedBy = "system"
                                };
                                _context.VendorMasters.Add(vendor);
                                _context.SaveChangesAsync();
                            }

                            // Need to check
                            var location = _context.LocationMasters.FirstOrDefault(l => l.City == asset.Location);
                            if (location == null && !String.IsNullOrEmpty(asset.Location))
                            {
                                location = new LocationMaster
                                {
                                    City = asset.Location,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = "system",
                                    ModifiedOn = DateTime.Now,
                                    ModifiedBy = "system"
                                };
                                _context.LocationMasters.Add(location);
                                _context.SaveChangesAsync();
                            }

                            // Extract numerical value for WarrantyPeriod
                            var warrantyPeriod = Regex.IsMatch(asset.Warranty, @"\d+(\.\d+)?") ? (int?)int.Parse(Regex.Match(asset.Warranty, @"\d+(\.\d+)?").Value) : null;

                            // Add valid record to assetMasterList
                            assetMasterList.Add(new AssetMaster
                            {
                                CompanyName = asset.Company,
                                AssetName = asset.Asset_Name,
                                AssetTag = asset.Asset_Tag,
                                Model = asset.Model_Name,
                                ModelNo = asset.Model_Number,
                                SerialNumber = asset.Serial_Number,
                                PurchasedDate = Convert.ToDateTime(asset.Purchase_Date), // Handle empty or invalid date
                                Cost = Convert.ToDecimal(asset.Purchase_Cost), // Handle empty or invalid cost
                                OrderNumber = asset.Order_Number,
                                Status = asset.Status,
                                WarrantyPeriod = warrantyPeriod,
                                WarrantyExpires = warrantyPeriod.HasValue ?
                                                        (asset.Purchase_Date != null ? Convert.ToDateTime(asset.Purchase_Date).AddMonths((int)warrantyPeriod.Value)
                                                                    : (DateTime?)null) : null, // Add warranty months to Purchase_Date if it exists
                                Notes = asset.Status,
                                AssetEntryProcess = "Bulk Upload",
                                CreatedOn = DateTime.Now,
                                CreatedBy = "", // Need to add logged-in user details
                                ModifiedOn = DateTime.Now,
                                ModifiedBy = "", // Need to add logged-in user details
                                CategoryID = category?.CategoryID,
                                ManufacturerID = vendor?.VendorID,
                                SupplierID = vendor?.VendorID,
                                DefaultLocationID = location?.LocationID
                            });
                        }
                        else
                        {
                            // Log the error and record the asset causing the issue
                            failedRecords.Add(new FailedAssetUploadModel { AssetDetails = asset, ErrorMessage = "Record Already present in DB." });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error and record the asset causing the issue
                        failedRecords.Add(new FailedAssetUploadModel { AssetDetails = asset, ErrorMessage = ex.Message });
                    }
                }

                // Save the valid records to the database
                if (assetMasterList.Count > 0)
                {
                    _context.AssetMasters.AddRange(assetMasterList);
                    _context.SaveChanges(); // Commit the valid records
                }

                success = true; // All records were processed successfully
                return assetMasterList.Count; // Return the count of successful records
            }
            catch (Exception ex)
            {
                success = false; // Indicate failure due to a global error
                Console.WriteLine($"Unhandled error: {ex.Message}"); // Log global error
                return 0; // No records were processed
            }
        }
    }
}
