using B2B_Subscription.Core.Entities;

namespace B2B_Subscription.Infrastructure.Data.Repositories.User
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetApplicationUserByIdAsync(string id);
        Task<ApplicationUser> UpdateApplicationUserAsync(ApplicationUser applicationUser);
    }
}