using InventoryHub.Enums;


namespace InventoryHub.Models
{
    // 🔹 Categoría de productos
    public class CategoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Relación 1:N con productos
        public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    }

    // 🔹 Producto genérico
    public class ProductEntity
    {
        public int Id { get; set; }
        
        //codigo propio
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string? Model { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        // Activo o descontinuado
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }

        // Aquí va el array/lista de imágenes
        public List<string> Images { get; set; } = new List<string>();
        // Fechas
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // 🔹 Relación con categoría
        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; } = null!;

        // 🔹 Detalles opcionales de tiras LED
        public LedStripDetailsEntity? LedDetails { get; set; }
    }

    // 🔹 Detalles específicos para tiras LED
    public class LedStripDetailsEntity
    {
        public int Id { get; set; }

        // 4A+4B  -> 4
        // 6T     -> 6
        public int StripCount { get; set; }

        // Normalmente 3V o 6V
        public string? LedVolts { get; set; }
        public int? LengthMm { get; set; }
        public int? LedCount { get; set; }
       
        // 4R+4L
        // 5A+5B
        // null cuando solo dicen "3T"
        public string? Distribution { get; set; }

        //cuadrado
        public LedType LedType { get; set; }

        // notas técnicas adicionales
        public string? Notes { get; set; }


        // Relación 1:N con modelos de TV compatibles
        public ICollection<TVModelEntity> CompatibleTVs { get; set; } = new List<TVModelEntity>();

        // FK hacia el producto
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; } = null!;
    }

    // 🔹 Modelo de TV compatible con la tira LED
    public class TVModelEntity
    {
        public int Id { get; set; }
        public string ModelCode { get; set; } = null!;

        // FK hacia LedStripDetails
        public int LedDetailsId { get; set; }
        public LedStripDetailsEntity LedDetails { get; set; } = null!;
    }
}