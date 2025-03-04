using PaymentEntity = B2B_Subscription.Core.Entities.Payment;

namespace B2B_Subscription.Infrastructure.Data.Repositories.Payment
{
    public interface IPaymentRepository
    {
        Task<PaymentEntity> CreatePaymentAsync(PaymentEntity payment);
        Task<IEnumerable<PaymentEntity>> GetPaymentsByUserIdAsync(string userId);

    }
}