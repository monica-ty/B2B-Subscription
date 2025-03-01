using B2B_Subscription.Core.Entities;

namespace B2B_Subscription.Infrastructure.Data.Repositories.User
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetUserProfileByIdAsync(Guid id);
        Task<UserProfile> GetUserProfileByUserIdAsync(string userId);
        Task<UserProfile> CreateUserProfileAsync(UserProfile userProfile);
        Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile);
        Task<bool> DeleteUserProfileAsync(Guid id);
    }
}