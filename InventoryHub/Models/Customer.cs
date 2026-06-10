using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(30)]
        public string DocumentNumber { get; set; }

        [StringLength(20)]
        public string DocumentType { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string Notes { get; set; }

    }
}
