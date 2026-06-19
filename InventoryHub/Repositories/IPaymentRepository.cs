using InventoryHub.Models;

namespace InventoryHub.Repositories
{
    public interface IPaymentRepository
    {
        Task<List<PaymentEntity>> GetBySaleIdAsync(int saleId);
        Task<PaymentEntity?> GetByIdAsync(int id);
        Task<PaymentEntity> AddAsync(PaymentEntity payment);
        Task<List<PaymentEntity>> GetByCustomerIdAsync(int customerId);
    }
}
