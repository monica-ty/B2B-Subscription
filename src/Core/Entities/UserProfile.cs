using System;
using System.ComponentModel.DataAnnotations;

namespace B2B_Subscription.Core.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Company { get; set; }
        public string? PhoneNumber { get; set; }
    }
} 