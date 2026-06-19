// CreateSaleRequestDTO.cs
using InventoryHub.Enums;

namespace InventoryHub.DTOs.Sale
{
    public class CreateSaleRequestDTO
    {
        public int? CustomerId { get; set; }
        public SaleDocumentType DocumentType { get; set; }
    }
}

// UpdateSaleRequestDTO.cs
namespace InventoryHub.DTOs.Sale
{
    public class UpdateSaleRequestDTO
    {
        public int? CustomerId { get; set; }
        public SaleDocumentType DocumentType { get; set; }
        public SaleStatus Status { get; set; }
        public string? ConsecutiveNumber { get; set; }
        public string? CUFE { get; set; }
        public string? DianStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

// AddToCartRequestDTO.cs
namespace InventoryHub.DTOs.Sale
{
    public class AddToCartRequestDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

// UpdateCartItemRequestDTO.cs
namespace InventoryHub.DTOs.Sale
{
    public class UpdateCartItemRequestDTO
    {
        public int Quantity { get; set; }
    }
}

// CheckoutRequestDTO.cs
namespace InventoryHub.DTOs.Sale
{
    public class CheckoutRequestDTO
    {
        public int? CustomerId { get; set; }
        public SaleDocumentType DocumentType { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

// SaleResponseDTO.cs
namespace InventoryHub.DTOs.Sale
{
    public class SaleResponseDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
        public SaleDocumentType DocumentType { get; set; }
        public SaleStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string? ConsecutiveNumber { get; set; }
        public string? CUFE { get; set; }
        public string? DianStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? DueDate { get; set; }
        public List<SaleDetailResponseDTO> Details { get; set; } = new();
    }
}

// SaleDetailResponseDTO.cs
namespace InventoryHub.DTOs.Sale
{
    public class SaleDetailResponseDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public int? WarrantyDays { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public bool WarrantyClaimed { get; set; }
        public DateTime? WarrantyClaimDate { get; set; }
        public string? WarrantyResolution { get; set; }
        public string? WarrantyNotes { get; set; }
        public string? WarrantyReplacementCode { get; set; }
    }
}

namespace InventoryHub.DTOs.Sale
{
    public class WarrantyClaimRequestDTO
    {
        public string Resolution { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? ReplacementCode { get; set; }
    }
}