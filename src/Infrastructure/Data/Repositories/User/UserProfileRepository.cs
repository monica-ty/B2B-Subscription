using B2B_Subscription.Core.Entities;
using B2B_Subscription.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace B2B_Subscription.Infrastructure.Data.Repositories.User
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly UserDbContext _context;

        public UserProfileRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> GetUserProfileByIdAsync(Guid id)
        {
            return await _context.UserProfiles.FindAsync(id);
        }

        public async Task<UserProfile> GetUserProfileByUserIdAsync(string userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task<UserProfile> CreateUserProfileAsync(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<bool> DeleteUserProfileAsync(Guid id)
        {
            var userProfile = await GetUserProfileByIdAsync(id);
            if (userProfile == null)
                return false;
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}