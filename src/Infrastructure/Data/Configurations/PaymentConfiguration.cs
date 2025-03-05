using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using B2B_Subscription.Core.Entities;
using PaymentEntity = B2B_Subscription.Core.Entities.Payment;

namespace B2B_Subscription.Infrastructure.Data.Configurations
{
    // Configuration class for Payment entity
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            // Configure primary key
            builder.HasKey(p => p.Id);

            // StripeSessionId is required and must be unique
            builder.Property(p => p.StripeSessionId)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false); // Stripe IDs are ASCII

            // UserId is required and must be a valid Guid
            builder.Property(p => p.UserId)
                .IsRequired()
                .HasConversion<Guid>();

            // Configure properties
            builder.Property(p => p.StripePaymentIntentId)
                .IsRequired(false)
                .HasMaxLength(50)
                .IsUnicode(false); // Stripe IDs are ASCII

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)"); // Common decimal format for currency

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20); // Status values: Succeeded/Pending/Failed

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Ensure StripeSessionId is unique
            builder.HasIndex(p => p.StripeSessionId)
                .IsUnique();
            // Ensure StripePaymentIntentId is unique
            builder.HasIndex(p => p.StripePaymentIntentId)
                .IsUnique()
                .HasFilter("[StripePaymentIntentId] IS NOT NULL");
        }
    }

    // Configuration class for PaymentMethod entity
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            // Configure primary key
            builder.HasKey(pm => pm.Id);

            // UserId is required and must be a valid Guid
            builder.Property(pm => pm.UserId)
                .IsRequired()
                .HasConversion<Guid>();

            // Configure properties
            builder.Property(pm => pm.StripePaymentMethodId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false); // Stripe IDs are ASCII

            builder.Property(pm => pm.Type)
                .IsRequired()
                .HasMaxLength(20); // Type values: card/paypal

            builder.Property(pm => pm.Last4)
                .HasMaxLength(4); // Last 4 digits of card/account

            builder.Property(pm => pm.Brand)
                .HasMaxLength(20); // Card brand (Visa, Mastercard, etc.)

            builder.Property(pm => pm.Expiry)
                .HasMaxLength(7); // Format: MM/YYYY

            // Ensure StripePaymentMethodId is unique
            builder.HasIndex(pm => pm.StripePaymentMethodId)
                .IsUnique();
        }
    }
}
