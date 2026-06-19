using InventoryHub.Enums;

namespace InventoryHub.DTOs.Sale
{
    public class RecordPaymentRequestDTO
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }

    public class PaymentResponseDTO
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}
