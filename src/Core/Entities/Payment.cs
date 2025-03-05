using System;
using System.ComponentModel.DataAnnotations;

namespace B2B_Subscription.Core.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string StripeSessionId { get; set; } = string.Empty;
        public string? StripePaymentIntentId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}