using System;
using System.ComponentModel.DataAnnotations;
namespace B2B_Subscription.Core.Entities
{
    public class License
    {
        public Guid Id { get; set; }
        [Required]
        public string SubscriptionId { get; set; } = string.Empty;
        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}