namespace InventoryHub.Models
{
    public class PaymentEntity
    {
        public int Id { get; set; }

        public int SaleId { get; set; }
        public virtual SaleEntity Sale { get; set; } = null!;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}
