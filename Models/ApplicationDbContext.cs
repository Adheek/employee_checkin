using Microsoft.EntityFrameworkCore;

namespace AEET.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add the DbSet properties using plural names
        public DbSet<RoleMaster> RoleMasters { get; set; } = default!;
        public DbSet<UserMaster> UserMasters { get; set; } = default!;
    }
}
