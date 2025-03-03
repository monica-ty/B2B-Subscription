using B2B_Subscription.Core.Entities;

namespace B2B_Subscription.Infrastructure.Data.Repositories.Subscription
{
    public interface IPlanRepository
    {
        Task<List<Plan>> GetAllPlansAsync();
        Task<Plan?> GetPlanByIdAsync(Guid id);
        Task<Plan> CreatePlanAsync(Plan plan);
        Task<Plan> UpdatePlanAsync(Plan plan);
        Task<bool> DeletePlanAsync(Guid id);
    }
    
}