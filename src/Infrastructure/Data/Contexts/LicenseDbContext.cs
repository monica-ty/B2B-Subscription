using Microsoft.EntityFrameworkCore;
using B2B_Subscription.Core.Entities;
using B2B_Subscription.Infrastructure.Data.Configurations;
namespace B2B_Subscription.Infrastructure.Data.Contexts
{
    public class LicenseDbContext : DbContext
    {
        public LicenseDbContext(DbContextOptions<LicenseDbContext> options) 
            : base(options) { }

        public DbSet<License> Licenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add configurations here
            modelBuilder.ApplyConfiguration(new LicenseConfiguration());
        }
    }
}   