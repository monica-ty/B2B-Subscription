using B2B_Subscription.Core.Entities;
using B2B_Subscription.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace B2B_Subscription.Infrastructure.Data.Repositories.User
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserDbContext _context;

        public ApplicationUserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetApplicationUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<ApplicationUser> UpdateApplicationUserAsync(ApplicationUser applicationUser)
        {
            _context.Users.Update(applicationUser);
            await _context.SaveChangesAsync();
            return applicationUser;
        }
    }
}
