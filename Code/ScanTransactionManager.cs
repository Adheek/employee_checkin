using System;
using System.Threading.Tasks;
using AEET.DTOs;
using AEET.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AEET.Code
{
    public class ScanTransactionManager
    {
        private readonly ApplicationDbContext _context;

        public ScanTransactionManager(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Processes a check-in transaction.
        /// </summary>
        public async Task ProcessCheckInAsync(ScanTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.AssetId))
                throw new ArgumentException("AssetId is required.", nameof(dto.AssetId));
            if (dto.EmployeeId <= 0)
                throw new ArgumentException("A valid EmployeeId is required.", nameof(dto.EmployeeId));

            var newScan = new ScanDetail
            {
                EmployeeId = dto.EmployeeId,
                AssetId = dto.AssetId,
                EmployeeName = dto.EmployeeName,
                TransactionTime = DateTime.UtcNow, // Use UTC for consistency
                TransactionType = "CheckIn",         // Set by server
                MismatchedEmployeeId = dto.MismatchedEmployeeId ?? string.Empty
            };

            _context.ScanDetails.Add(newScan);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Processes a check-out transaction.
        /// </summary>
        public async Task ProcessCheckOutAsync(ScanTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.AssetId))
                throw new ArgumentException("AssetId is required.", nameof(dto.AssetId));
            if (dto.EmployeeId <= 0)
                throw new ArgumentException("A valid EmployeeId is required.", nameof(dto.EmployeeId));

            var newScan = new ScanDetail
            {
                EmployeeId = dto.EmployeeId,
                AssetId = dto.AssetId,
                EmployeeName = dto.EmployeeName,
                TransactionTime = DateTime.UtcNow, // Use UTC for consistency
                TransactionType = "CheckOut",
                MismatchedEmployeeId = string.Empty // For check-out, set as empty
            };

            _context.ScanDetails.Add(newScan);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all scan logs (without any time filtering).
        /// </summary>
        public async Task<object> GetAllScanLogsAsync()
        {
            var logs = await (from scanDetail in _context.ScanDetails
                              join assetDetail in _context.AssetMasters
                                on scanDetail.AssetId equals assetDetail.AssetID into assetGroup
                              from asset in assetGroup.DefaultIfEmpty() // Left join
                              orderby scanDetail.TransactionTime descending
                              select new
                              {
                                  scanDetail.ScanId,
                                  scanDetail.AssetId,
                                  scanDetail.EmployeeName,
                                  scanDetail.TransactionTime,
                                  scanDetail.TransactionType,
                                  scanDetail.EmployeeId,
                                  AssetName = asset != null ? asset.AssetName : null
                              }).ToListAsync();

            return logs;
        }

        /// <summary>
        /// Retrieves transaction logs for a given asset/employee (without a time filter).
        /// </summary>
        public async Task<object> GetAllTransaction(ScanTransactionDto dto)
        {
            var logs = await (from scanDetail in _context.ScanDetails
                              join assetDetail in _context.AssetMasters
                                on scanDetail.AssetId equals assetDetail.AssetID into assetGroup
                              from asset in assetGroup.DefaultIfEmpty() // Left join
                              where scanDetail.AssetId == dto.AssetId &&
                                    scanDetail.EmployeeId == dto.EmployeeId &&
                                    scanDetail.EmployeeName == dto.EmployeeName
                              orderby scanDetail.TransactionTime descending
                              select new
                              {
                                  scanDetail.ScanId,
                                  scanDetail.AssetId,
                                  scanDetail.EmployeeName,
                                  scanDetail.TransactionTime,
                                  scanDetail.TransactionType,
                                  scanDetail.EmployeeId,
                                  AssetName = asset != null ? asset.AssetName : null,
                                  Status = asset != null ? asset.Status : null
                              }).ToListAsync();

            return logs;
        }

        /// <summary>
        /// Processes a scanned transaction.
        /// If the last transaction for the same asset/employee was a CheckIn within 17 hours, then process a CheckOut.
        /// Otherwise, process a CheckIn.
        /// </summary>
        public async Task ProcessScanAsync(ScanTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.AssetId))
                throw new ArgumentException("AssetId is required.", nameof(dto.AssetId));
            if (dto.EmployeeId <= 0)
                throw new ArgumentException("A valid EmployeeId is required.", nameof(dto.EmployeeId));

            // Retrieve the most recent record for this asset/employee combination
            var lastRecord = await _context.ScanDetails
                .Where(s => s.AssetId == dto.AssetId &&
                            s.EmployeeId == dto.EmployeeId &&
                            s.EmployeeName == dto.EmployeeName)
                .OrderByDescending(s => s.TransactionTime)
                .FirstOrDefaultAsync();

            DateTime now = DateTime.UtcNow;
            if (lastRecord != null)
            {
                // Ensure TransactionTime has a value
                if (!lastRecord.TransactionTime.HasValue)
                {
                    await ProcessCheckInAsync(dto);
                    return;
                }
                var diffHours = (now - lastRecord.TransactionTime.Value).TotalHours;
                // If the last record was a CheckIn and within 17 hours, then process CheckOut.
                if (lastRecord.TransactionType == "CheckIn" && diffHours < 17)
                {
                    await ProcessCheckOutAsync(dto);
                    return;
                }
            }

            // Otherwise, process a CheckIn.
            await ProcessCheckInAsync(dto);
        }
    }
}
