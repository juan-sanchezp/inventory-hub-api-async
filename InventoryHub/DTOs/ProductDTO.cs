// ADD dataAnnotations.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryHub.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = null!;

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

        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        [Range(0, 10000)]
        public int MinStock { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Aquí va el array/lista de imágenes
        public List<string> Images { get; set; } = new List<string>();

        // Detalles opcionales de LED
        public LedStripDetailsDTO? LedDetails { get; set; }
    }

    public class LedStripDetailsDTO
    {
        public int? LengthMm { get; set; }
        public int? LedCount { get; set; }

        //public string? LedType { get; set; } cuadrado, normal, sin lente

        [StringLength(10)]
        [RegularExpression(@"^\d{1,2}V$", ErrorMessage = "Formato válido: 3V, 6V")]
        public string? Ledvolts { get; set; }


        // Modelos de TV compatibles
        public List<string>? CompatibleTVModels { get; set; }
    }

    // DTO para categoría
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}