namespace InventoryHub.Models
{
    public class ProductEntity
    {
        public int Id { get; set; }

        // Código único del producto (ej: H32-4)
        public string Code { get; set; } = null!;

        // Campos básicos
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string? Model { get; set; }
        public float Price { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }

        // Campos opcionales para tiras LED
        public int? LengthMm { get; set; }
        public int? LedCount { get; set; }
    }
}

