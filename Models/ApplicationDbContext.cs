using Microsoft.EntityFrameworkCore;

namespace AEET.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing DbSet properties
        public DbSet<RoleMaster> RoleMasters { get; set; } = default!;
        public DbSet<UserMaster> UserMasters { get; set; } = default!;
        public DbSet<AssetMaster> AssetMasters { get; set; } = default!;

        // New DbSet properties for additional tables
        public DbSet<CategoryMaster> CategoryMasters { get; set; } = default!;
        public DbSet<VendorMaster> VendorMasters { get; set; } = default!;
        public DbSet<LocationMaster> LocationMasters { get; set; } = default!;

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAssetMapping> EmployeeAssetMappings { get; set; }
    }
}
