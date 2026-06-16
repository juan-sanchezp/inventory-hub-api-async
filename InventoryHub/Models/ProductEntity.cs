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

        public string Barcode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string? Model { get; set; }
        public decimal Price { get; set; }
        public decimal? TaxRate { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        // Activo o descontinuado
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }

        // Aquí va el array/lista de imágenes
        //public List<string> Images { get; set; } = new List<string>();
        public ICollection<ProductImageEntity> Images { get; set; } = new List<ProductImageEntity>();

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

        public int Inch { get; set; } // <- 22, 32, ..

        // 4A+4B  -> 4
        // 6T     -> 6
        public int StripCount { get; set; }

        public int? LengthMm { get; set; }
        public int? LedCount { get; set; }

        // Normalmente 3V o 6V
        public int? LedVolts { get; set; }

        //"4708-K50DFG-A3113N01",
        public string BoardCode { get; set; }
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

    // 🔹 Entidad para las imágenes de productos
    public class ProductImageEntity
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;        // URL de Cloudinary
        public string PublicId { get; set; } = null!;   // public_id para borrar/reemplazar
        public bool IsMain { get; set; } = false;       // Imagen principal

        // FK hacia el producto
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; } = null!;
    }
}