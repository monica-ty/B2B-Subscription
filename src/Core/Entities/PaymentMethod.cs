using System;
using System.ComponentModel.DataAnnotations;

namespace B2B_Subscription.Core.Entities
{
   public class PaymentMethod
    {
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string StripePaymentMethodId { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;
        public string? Last4 { get; set; }
        public string? Brand { get; set; }
        public string? Expiry { get; set; }
    }

}