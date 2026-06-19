using InventoryHub.Enums;

namespace InventoryHub.Models
{
    public class SaleEntity
    {
        public int Id { get; set; }

        // Datos generales de la venta
        public int? CustomerId { get; set; }
        public virtual CustomerEntity? Customer { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public SaleDocumentType DocumentType { get; set; }  // Pos = 1, Electronic = 2
        public SaleStatus Status { get; set; }  // Draft = 1, Confirmed = 2, Completed = 3, Cancelled = 4

        // ========== ESTADO DE PAGO ==========
        public PaymentStatus PaymentStatus { get; set; }  // Pending = 1, Partial = 2, Paid = 3, Overdue = 4, Cancelled = 5

        // Totales (calculados desde los detalles)
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        // ========== CAMPOS EXCLUSIVOS PARA FACTURACIÓN ELECTRÓNICA ==========

        // Numeración y control DIAN
        public string? ConsecutiveNumber { get; set; }
        public string? CUFE { get; set; }
        public string? DianStatus { get; set; }
        public DateTime? DianSentDate { get; set; }
        public DateTime? DianApprovedDate { get; set; }
        public string? DianResponseMessage { get; set; }

        // Datos de pago y comerciales
        public string? PaymentMethod { get; set; }
        public DateTime? DueDate { get; set; }
        public string? PurchaseOrderNumber { get; set; }

        // Archivos generados
        public string? XmlContent { get; set; }
        public string? PdfUrl { get; set; }

        // Relaciones
        public virtual ICollection<SaleDetailEntity> Details { get; set; } = new List<SaleDetailEntity>();
        public virtual ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();
    }
}
