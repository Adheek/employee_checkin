using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AEET.Models;
using Microsoft.EntityFrameworkCore;

namespace AEET.Code
{
    public class QrGeneratorManager
    {
        private readonly ApplicationDbContext _context;

        public QrGeneratorManager(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all assets whose AssetName contains the given term.
        /// </summary>
        public async Task<List<AssetMaster>> SearchByAssetNameAsync(string name)
        {
            return await _context.AssetMasters
                .Where(a => EF.Functions.Like(a.AssetName, $"%{name}%"))
                .ToListAsync();
        }
    }
}
