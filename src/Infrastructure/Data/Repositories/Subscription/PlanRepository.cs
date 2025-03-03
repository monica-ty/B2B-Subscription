using B2B_Subscription.Core.Entities;
using B2B_Subscription.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace B2B_Subscription.Infrastructure.Data.Repositories.Subscription
{
    public class PlanRepository : IPlanRepository
    {
        private readonly SubscriptionDbContext _context;

        public PlanRepository(SubscriptionDbContext context)
        {
            _context = context;
        }

        public async Task<List<Plan>> GetAllPlansAsync()
        {
            return await _context.Plans.ToListAsync();
        }

        public async Task<Plan?> GetPlanByIdAsync(Guid id)
        {
            return await _context.Plans.FindAsync(id);
        }

        public async Task<Plan> CreatePlanAsync(Plan plan)
        {
            await _context.Plans.AddAsync(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<Plan> UpdatePlanAsync(Plan plan)
        {
            _context.Plans.Update(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<bool> DeletePlanAsync(Guid id)
        {
            var plan = await GetPlanByIdAsync(id);
            if (plan == null)
                return false;
            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}