using Microsoft.AspNetCore.Identity;
using B2B_Subscription.Core.Entities;

public class NoOpEmailSender : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        // For development, just log or do nothing
        Console.WriteLine($"Confirmation link for {email}: {confirmationLink}");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        // For development, just log or do nothing
        Console.WriteLine($"Password reset link for {email}: {resetLink}");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        // For development, just log or do nothing
        Console.WriteLine($"Password reset code for {email}: {resetCode}");
        return Task.CompletedTask;
    }
} 