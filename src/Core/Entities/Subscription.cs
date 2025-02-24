using System;
using System.ComponentModel.DataAnnotations;
namespace B2B_Subscription.Core.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]  
        public Guid PlanId { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
} 