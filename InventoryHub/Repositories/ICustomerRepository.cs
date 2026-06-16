using InventoryHub.Models;

namespace InventoryHub.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<CustomerEntity>> GetAllAsync();

        Task<CustomerEntity?> GetByIdAsync(int id);

        Task<CustomerEntity?> AddAsync(CustomerEntity customerEntity);

        Task<CustomerEntity> UpdateAsync(CustomerEntity customerEntity);

        Task<bool> DeleteAsync(CustomerEntity customerEntity);
    }
}