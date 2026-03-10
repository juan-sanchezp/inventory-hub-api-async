using InventoryHub.Data;
using InventoryHub.DTOs;
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
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<ProductEntity>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.LedDetails)
                    .ThenInclude(l => l.CompatibleTVs)
                .ToListAsync();
            //return await _context.Products.ToListAsync();
        }

        public async Task<ProductEntity?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.LedDetails)
                    .ThenInclude(l => l.CompatibleTVs)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductEntity> UpdateAsync(ProductEntity productEntity)
        {
            _context.Products.Update(productEntity);
            await _context.SaveChangesAsync();
            return productEntity;
        }

        public async Task<List<ProductEntity>> SearchLedStripsAsync(LedStripFilter filter)
        {
            var query = _context.Products
                .Include(p => p.LedDetails)
                    .ThenInclude(ld => ld.CompatibleTVs)
                .Include(p => p.Category)
                .Where(p => p.Category.Name == "Tiras LED")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.CompatibleTVModel))
                query = query.Where(p =>
                    p.LedDetails != null &&
                    p.LedDetails.CompatibleTVs.Any(tv =>
                        tv.ModelCode.Contains(filter.CompatibleTVModel)));

            if (filter.MinLedCount.HasValue)
                query = query.Where(p =>
                    p.LedDetails != null &&
                    p.LedDetails.LedCount >= filter.MinLedCount.Value);

            if (filter.MaxLedCount.HasValue)
                query = query.Where(p =>
                    p.LedDetails != null &&
                    p.LedDetails.LedCount <= filter.MaxLedCount.Value);

            if (filter.MinLengthMm.HasValue)
                query = query.Where(p =>
                    p.LedDetails != null &&
                    p.LedDetails.LengthMm >= filter.MinLengthMm.Value);

            if (filter.MaxLengthMm.HasValue)
                query = query.Where(p =>
                    p.LedDetails != null &&
                    p.LedDetails.LengthMm <= filter.MaxLengthMm.Value);

            if (!string.IsNullOrWhiteSpace(filter.LedVolts))
            {
                var volts = filter.LedVolts.Trim().ToLower();

                query = query.Where(p =>
                    p.LedDetails != null &&
                    p.LedDetails.LedVolts.ToLower().Contains(volts));
            }

            return await query.ToListAsync();
        }
    }
}