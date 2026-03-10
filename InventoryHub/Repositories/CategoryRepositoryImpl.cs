using InventoryHub.Data;
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Repositories
{
    public class CategoryRepositoryImpl : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryEntity?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CategoryEntity> AddAsync(CategoryEntity category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
