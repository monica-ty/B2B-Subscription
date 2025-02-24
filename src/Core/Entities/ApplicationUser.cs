using Microsoft.AspNetCore.Identity;

namespace B2B_Subscription.Core.Entities
{
    // Custom ApplicationUser class that extends IdentityUser to add Stripe-specific properties
    public class ApplicationUser : IdentityUser 
    {
        // Property to store the Stripe Customer ID
        // This allows us to link our application users with their Stripe customer accounts
        public string? StripeCustomerId { get; set; }
    }
}
