using InventoryHub.Models;
using System.Threading.Tasks;

namespace InventoryHub.Repositories
{
    public interface IProductRepository
    {
        Task<List<ProductEntity>> GetAllAsync();
        Task<ProductEntity?> GetByIdAsync(int id);
        Task<ProductEntity?> AddAsync(ProductEntity productEntity);
        Task<ProductEntity> UpdateAsync(ProductEntity productEntity);
        Task<bool> DeleteAsync(ProductEntity productEntity);
    }
}