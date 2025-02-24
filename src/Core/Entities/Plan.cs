    
using System;

namespace B2B_Subscription.Core.Entities
{
    public class Plan
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? StripePriceId { get; set; }
        public string? BillingCycle { get; set; }
        public decimal Price { get; set; }
        public int MaxLicenses { get; set; }
    }
}
