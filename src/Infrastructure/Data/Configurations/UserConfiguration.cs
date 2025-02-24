using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using B2B_Subscription.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace B2B_Subscription.Infrastructure.Data.Configurations
{
    // Configuration class for UserProfile entity
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // Configure primary key
            builder.HasKey(u => u.Id);

            // UserId is required and must be a valid Guid
            builder.Property(u => u.UserId)
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            // Configure properties
            builder.Property(u => u.FullName)
                .HasMaxLength(100); // Set reasonable max length for full name

            builder.Property(u => u.Company)
                .HasMaxLength(100); // Set reasonable max length for company name

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20); // Set reasonable max length for phone number

            // Configure relationship with AspNetUsers (ApplicationUser)
            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<UserProfile>(up => up.UserId)
                .HasPrincipalKey<ApplicationUser>(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    // Configuration class for IdentityUser customization
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Configure additional properties for AspNetUsers table
            builder.Property<DateTime>("CreatedAt")
                .HasDefaultValueSql("GETDATE()");

            builder.Property<string>("StripeCustomerId")
                .HasMaxLength(50)
                .IsUnicode(false); // Since Stripe IDs are ASCII

            // Ensure StripeCustomerId is unique when not null
            builder.HasIndex(u => u.StripeCustomerId)
                .IsUnique()
                .HasFilter("[StripeCustomerId] IS NOT NULL");
        }
    }
}
