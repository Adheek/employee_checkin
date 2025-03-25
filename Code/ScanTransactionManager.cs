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
        /// Retrieves all scan logs.
        /// </summary>
        public async Task<object> GetAllScanLogsAsync()
        {
            var logs = await _context.ScanDetails
                .Select(s => new
                {
                    s.ScanId,
                    s.AssetId,
                    s.EmployeeName,
                    s.TransactionTime,
                    s.TransactionType,
                    s.EmployeeId
                })
                .ToListAsync();

            return logs;
        }
    }
}
