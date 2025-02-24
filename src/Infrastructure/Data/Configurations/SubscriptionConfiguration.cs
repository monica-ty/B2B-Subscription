using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using B2B_Subscription.Core.Entities;

namespace B2B_Subscription.Infrastructure.Data.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(x => x.Id);

            // UserId is required and must be a valid Guid
            builder.Property(x => x.UserId)
                .IsRequired()
                .HasConversion<Guid>();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PlanId)
                .IsRequired()
                .HasConversion<Guid>();

            builder.HasOne<Plan>()
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 