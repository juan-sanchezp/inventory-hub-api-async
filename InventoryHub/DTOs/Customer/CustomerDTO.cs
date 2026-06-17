using System.ComponentModel.DataAnnotations;

namespace InventoryHub.DTOs.Customer
{
    public class CustomerDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string DocumentNumber { get; set; } = string.Empty;

        [StringLength(20)]
        public string? DocumentType { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

