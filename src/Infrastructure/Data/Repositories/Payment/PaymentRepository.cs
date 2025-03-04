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
    }
}