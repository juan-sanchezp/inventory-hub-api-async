using InventoryHub.Models;

namespace InventoryHub.Repositories
{
    public interface ICategoryRepository
    {
        Task<CategoryEntity?> GetByIdAsync(int id);
        Task<CategoryEntity> AddAsync(CategoryEntity category);
    }
}
