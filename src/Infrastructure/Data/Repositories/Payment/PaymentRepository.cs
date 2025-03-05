using PaymentEntity = B2B_Subscription.Core.Entities.Payment;
using B2B_Subscription.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace B2B_Subscription.Infrastructure.Data.Repositories.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentEntity> CreatePaymentAsync(PaymentEntity payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<PaymentEntity>> GetPaymentsByUserIdAsync(string userId)
        {
            return await _context.Payments.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<PaymentEntity> GetPaymentByStripeSessionIdAsync(string stripeSessionId)
        {
            if (string.IsNullOrEmpty(stripeSessionId))
            {
                throw new ArgumentNullException(nameof(stripeSessionId));
            }
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.StripeSessionId == stripeSessionId);
            if (payment == null)
            {
                throw new Exception("Payment not found");
            }
            return payment;
        }

        public async Task<PaymentEntity> UpdatePaymentAsync(PaymentEntity payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

    }
}