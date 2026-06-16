
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
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<SaleEntity> Sales { get; set; }
        public DbSet<SaleDetailEntity> SaleDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()!.ToLowerInvariant());
            }
        }
    }
}