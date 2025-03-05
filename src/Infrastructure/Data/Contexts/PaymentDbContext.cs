using Microsoft.EntityFrameworkCore;
using B2B_Subscription.Core.Entities;
using PaymentEntity = B2B_Subscription.Core.Entities.Payment;
using B2B_Subscription.Infrastructure.Data.Configurations;

namespace B2B_Subscription.Infrastructure.Data.Contexts
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) 
            : base(options) { }

        public DbSet<PaymentEntity> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add configurations here
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodConfiguration());
        }
    }
}