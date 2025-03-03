namespace B2B_Subscription.Core.DTOs
{
    public class CreatePlanDto
    {
        public string Name { get; set; } = string.Empty;
        public string? StripePriceId { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MaxLicenses { get; set; }
    }

    public class UpdatePlanDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? StripePriceId { get; set; }
        public string? BillingCycle { get; set; }
        public decimal? Price { get; set; }
        public int? MaxLicenses { get; set; }
    }
}