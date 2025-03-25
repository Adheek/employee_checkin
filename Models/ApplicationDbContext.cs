using Microsoft.EntityFrameworkCore;
using AEET.Models.DTOs;

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

        public DbSet<ScanDetail> ScanDetails { get; set; } = default!;


        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAssetMapping> EmployeeAssetMappings { get; set; }
        
        // **Add the DbSet for the DTO**
        public DbSet<MappedAssetDto> MappedAssetDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AssetMaster.AssetID as a database-generated column using your default SQL expression.
            modelBuilder.Entity<AssetMaster>()
                .Property(a => a.AssetID)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("('AST-' + RIGHT('000' + CAST(NEXT VALUE FOR dbo.Seq_AssetID AS VARCHAR(3)), 3))");

            // Configure Employee mapping if needed...
            // (For example, using EmployeeId as the key and ignoring EmpId if desired)

            // Configure the DTO as a keyless entity.
            modelBuilder.Entity<MappedAssetDto>().HasNoKey();
        }
    }
}
