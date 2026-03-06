using InventoryHub.Data;
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryHub.Repositories
{
    public class ProductRepositoryImpl : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductEntity?> AddAsync(ProductEntity productEntity)
        {
            // Validación de duplicado async
            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == productEntity.Name);

            if (existing != null)
                return null; // ya existe

            await _context.Products.AddAsync(productEntity);
            await _context.SaveChangesAsync();

            return productEntity;
        }

        public async Task<bool> DeleteAsync(ProductEntity productEntity)
        {
            _context.Products.Remove(productEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductEntity>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<ProductEntity?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductEntity> UpdateAsync(ProductEntity productEntity)
        {
            _context.Products.Update(productEntity);
            await _context.SaveChangesAsync();
            return productEntity;
        }
    }
}