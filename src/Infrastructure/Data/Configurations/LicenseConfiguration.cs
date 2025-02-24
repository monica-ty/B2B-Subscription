using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using B2B_Subscription.Core.Entities;

namespace B2B_Subscription.Infrastructure.Data.Configurations
{
    // Configuration class for License entity
    public class LicenseConfiguration : IEntityTypeConfiguration<License>
    {
        public void Configure(EntityTypeBuilder<License> builder)
        {
            // Configure primary key
            builder.HasKey(l => l.Id);

            // Configure properties
            builder.Property(l => l.SubscriptionId)
                .IsRequired(); // Foreign key to Subscription entity (Guid type)

            builder.Property(l => l.Key)
                .IsRequired()
                .HasMaxLength(100); // Set reasonable max length for license key

            builder.Property(l => l.UserId)
                .IsRequired()
                .HasConversion<Guid>();

            builder.Property(l => l.Status)
                .IsRequired()
                .HasMaxLength(20); // Status values: Active/Revoked

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Ensure Key is unique
            builder.HasIndex(l => l.Key)
                .IsUnique();
        }
    }
}
