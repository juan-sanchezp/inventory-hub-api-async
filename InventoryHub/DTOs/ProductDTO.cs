// ADD dataAnnotations.
using System.ComponentModel.DataAnnotations;

namespace InventoryHub.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20)]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(50)]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50)]
        public string Brand { get; set; } = null!;

        [StringLength(50)]
        public string? Model { get; set; }

        [Range(0.01, 100000)]
        public float Price { get; set; }

        public int Stock { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Campos opcionales tiras LED
        public int? LengthMm { get; set; }
        public int? LedCount { get; set; }
    }


    
}
