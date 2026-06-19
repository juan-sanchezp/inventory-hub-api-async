using InventoryHub.Data;
using InventoryHub.DTOs.Sale;
using InventoryHub.Enums;
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Repositories
{
    public class SaleRepositoryImpl : ISaleRepository
    {
        private readonly AppDbContext _context;

        public SaleRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        // ==================== BASIC CRUD ====================

        public async Task<SaleEntity?> AddAsync(SaleEntity saleEntity)
        {
            await _context.Sales.AddAsync(saleEntity);
            await _context.SaveChangesAsync();
            return saleEntity;
        }

        public async Task<bool> DeleteAsync(SaleEntity saleEntity)
        {
            _context.Sales.Remove(saleEntity);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<SaleEntity>> GetAllAsync(SaleFilterDTO? filter = null)
        {
            var query = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Product)
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.ProductCode))
                    query = query.Where(s => s.Details.Any(d => d.Product != null && d.Product.Code.Contains(filter.ProductCode)));

                if (!string.IsNullOrWhiteSpace(filter.CustomerName))
                    query = query.Where(s => s.Customer != null && s.Customer.Name.Contains(filter.CustomerName));

                if (filter.StartDate.HasValue)
                    query = query.Where(s => s.SaleDate >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(s => s.SaleDate <= filter.EndDate.Value.AddDays(1));
            }

            return await query
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<SaleEntity?> GetByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SaleEntity> UpdateAsync(SaleEntity saleEntity)
        {
            _context.Sales.Update(saleEntity);
            await _context.SaveChangesAsync();
            return saleEntity;
        }

        // ==================== CART SPECIFIC ====================

        public async Task<SaleEntity?> GetCartAsync()
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Status == SaleStatus.Draft);
        }

        // ==================== SALE DETAIL OPERATIONS ====================

        public async Task<SaleDetailEntity?> GetDetailByIdAsync(int id)
        {
            return await _context.SaleDetails
                .Include(d => d.Product)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<SaleDetailEntity>> GetDetailsBySaleIdAsync(int saleId)
        {
            return await _context.SaleDetails
                .Include(d => d.Product)
                .Where(d => d.SaleId == saleId)
                .ToListAsync();
        }

        public async Task<SaleDetailEntity> AddDetailAsync(SaleDetailEntity detail)
        {
            await _context.SaleDetails.AddAsync(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        public async Task<SaleDetailEntity> UpdateDetailAsync(SaleDetailEntity detail)
        {
            _context.SaleDetails.Update(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        public async Task<bool> DeleteDetailAsync(SaleDetailEntity detail)
        {
            _context.SaleDetails.Remove(detail);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ClearDetailsAsync(int saleId)
        {
            var details = await _context.SaleDetails
                .Where(d => d.SaleId == saleId)
                .ToListAsync();

            _context.SaleDetails.RemoveRange(details);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        // ==================== PAYMENT BALANCE HELPERS ====================

        public async Task<List<SaleEntity>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Sales
                .Where(s => s.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task UpdateCustomerBalance(int customerId, decimal balance)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer != null)
            {
                customer.CurrentBalance = balance;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PaymentEntity> AddPaymentAsync(PaymentEntity payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        // ==================== PRODUCT HELPERS ====================

        public async Task<ProductEntity?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<bool> UpdateProductStock(int productId, int quantityToSubtract)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return false;
            if (product.Stock < quantityToSubtract) return false;

            product.Stock -= quantityToSubtract;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}