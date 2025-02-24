using Microsoft.EntityFrameworkCore;
using B2B_Subscription.Core.Entities;
using B2B_Subscription.Infrastructure.Data.Configurations;

namespace B2B_Subscription.Infrastructure.Data.Contexts
{
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) 
            : base(options) { }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Plan> Plans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add configurations here
            modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        }
    }
} 