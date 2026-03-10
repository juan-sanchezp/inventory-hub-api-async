// ADD dataAnnotations.
using InventoryHub.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryHub.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = null!;
        public string Barcode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!; // para mostrar en GET

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = null!;

        [StringLength(50)]
        public string? Model { get; set; }

        [Range(0.01, 100000000)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        [Range(0, 10000)]
        public int MinStock { get; set; }
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        // Aquí va el array/lista de imágenes
        public List<ProductImageDTO> Images { get; set; } = new List<ProductImageDTO>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Detalles opcionales de LED
        public LedStripDetailsDTO? LedDetails { get; set; }
    }

    public class LedStripDetailsDTO
    {
        public int Inch { get; set; } // <- NUEVO CAMPO

        [Range(1, 50)]
        public int StripCount { get; set; }

        public int? LengthMm { get; set; }

        public int? LedCount { get; set; }

        [Range(3, 6)]
        public int? LedVolts { get; set; }

        [StringLength(20)]
         
        public string BoardCode { get; set; }
        public string? Distribution { get; set; }
        public LedType LedType { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }


        // Modelos de TV compatibles
        public List<string>? CompatibleTVModels { get; set; }
    }

    // DTO para categoría
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class ProductImageDTO
    {
        public string Url { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public bool IsMain { get; set; }
    }

    //DTO UpdateStock
    public class UpdateStockDTO
    {
        public int Id { get; set; }
        public int Stock { get; set; }
    }
}