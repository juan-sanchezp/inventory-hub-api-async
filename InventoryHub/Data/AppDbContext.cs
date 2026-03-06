
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // definition of tables
        public DbSet<ProductEntity> Products { get; set; }
    }
}