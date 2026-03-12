using InventoryHub.Data;
using InventoryHub.DTOs;
using InventoryHub.Enums;
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
        public async Task<List<ProductEntity>> GetAllOrdenAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.LedDetails)
                    .ThenInclude(l => l.CompatibleTVs)
                .ToListAsync();

            var ordered = products.OrderBy(p => p.Code, new NaturalStringComparer()).ToList();
            return ordered;
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
                .AsNoTracking()
                .Include(p => p.LedDetails)
                    .ThenInclude(ld => ld.CompatibleTVs)
                .Include(p => p.Category)
                .Where(p => p.Category.Name == "Tiras LED" && p.LedDetails != null)
                .AsQueryable();

            // 🔎 Búsqueda general
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Code.Contains(search) ||
                    p.Barcode.Contains(search) ||
                    (p.Model != null && p.Model.Contains(search)));
            }

            // 🔎 Modelo de TV compatible
            if (!string.IsNullOrWhiteSpace(filter.CompatibleTVModel))
            {
                var model = filter.CompatibleTVModel.Trim();

                query = query.Where(p =>
                    p.LedDetails!.CompatibleTVs.Any(tv =>
                        tv.ModelCode.Contains(model)));
            }

            // 📏 Pulgadas
            if (filter.Inch.HasValue)
                query = query.Where(p =>
                    p.LedDetails!.Inch == filter.Inch.Value);

            // 🔢 Cantidad de tiras
            if (filter.StripCount.HasValue)
                query = query.Where(p =>
                    p.LedDetails!.StripCount == filter.StripCount.Value);

            // ⚡ Voltaje
            if (filter.LedVolts.HasValue)
                query = query.Where(p =>
                    p.LedDetails!.LedVolts == filter.LedVolts.Value);

            // 💡 Cantidad de LEDs
            if (filter.LedCount.HasValue)
                query = query.Where(p =>
                    p.LedDetails!.LedCount == filter.LedCount.Value);

            if (filter.LedType.HasValue)
            {
                var ledType = (LedType)filter.LedType.Value;

                query = query.Where(p =>
                    p.LedDetails!.LedType == ledType);
            }

            // 🔧 BoardCode
            if (!string.IsNullOrWhiteSpace(filter.BoardCode))
            {
                var board = filter.BoardCode.Trim();

                query = query.Where(p =>
                    p.LedDetails!.BoardCode.Contains(board));
            }

            return await query.ToListAsync();
        }


    }
}