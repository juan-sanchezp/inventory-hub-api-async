using InventoryHub.Data;
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Repositories
{
    public class PaymentRepositoryImpl : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentEntity>> GetBySaleIdAsync(int saleId)
        {
            return await _context.Payments
                .Where(p => p.SaleId == saleId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<PaymentEntity?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PaymentEntity> AddAsync(PaymentEntity payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<PaymentEntity>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Payments
                .Include(p => p.Sale)
                .Where(p => p.Sale.CustomerId == customerId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}
