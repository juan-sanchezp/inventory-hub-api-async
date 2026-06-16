using InventoryHub.Data;
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Repositories
{
    public class CustomerRepositoryImpl : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerEntity?> AddAsync(CustomerEntity customerEntity)
        {
            // Validación de cliente duplicado por documento
            var existing = await _context.Customers
                .FirstOrDefaultAsync(c => c.DocumentNumber == customerEntity.DocumentNumber);

            if (existing != null)
                return null;

            await _context.Customers.AddAsync(customerEntity);
            await _context.SaveChangesAsync();

            return customerEntity;
        }

        public async Task<bool> DeleteAsync(CustomerEntity customerEntity)
        {
            _context.Customers.Remove(customerEntity);

            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<List<CustomerEntity>> GetAllAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CustomerEntity?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CustomerEntity> UpdateAsync(CustomerEntity customerEntity)
        {
            _context.Customers.Update(customerEntity);

            await _context.SaveChangesAsync();

            return customerEntity;
        }
    }
}