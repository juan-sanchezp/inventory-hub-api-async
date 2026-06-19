using InventoryHub.DTOs.Sale;
using InventoryHub.Models;

namespace InventoryHub.Repositories
{
    public interface ISaleRepository
    {
        // Basic CRUD
        Task<List<SaleEntity>> GetAllAsync(SaleFilterDTO? filter = null);
        Task<SaleEntity?> GetByIdAsync(int id);
        Task<SaleEntity?> AddAsync(SaleEntity saleEntity);
        Task<SaleEntity> UpdateAsync(SaleEntity saleEntity);
        Task<bool> DeleteAsync(SaleEntity saleEntity);

        // Cart specific
        Task<SaleEntity?> GetCartAsync();  // Obtener Sale con Status = Draft

        // SaleDetail operations
        Task<SaleDetailEntity?> GetDetailByIdAsync(int id);
        Task<List<SaleDetailEntity>> GetDetailsBySaleIdAsync(int saleId);
        Task<SaleDetailEntity> AddDetailAsync(SaleDetailEntity detail);
        Task<SaleDetailEntity> UpdateDetailAsync(SaleDetailEntity detail);
        Task<bool> DeleteDetailAsync(SaleDetailEntity detail);
        Task<bool> ClearDetailsAsync(int saleId);

        // Product helpers
        Task<ProductEntity?> GetProductByIdAsync(int productId);
        Task<bool> UpdateProductStock(int productId, int quantityToSubtract);

        // Payment balance helpers
        Task<List<SaleEntity>> GetByCustomerIdAsync(int customerId);
        Task UpdateCustomerBalance(int customerId, decimal balance);
        Task<PaymentEntity> AddPaymentAsync(PaymentEntity payment);
    }
}