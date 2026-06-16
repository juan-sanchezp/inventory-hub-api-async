using InventoryHub.Enums;

namespace InventoryHub.Models
{
    public class SaleEntity
    {
        public int Id { get; set; }

        // Datos generales de la venta
        public int? CustomerId { get; set; }
        public virtual CustomerEntity? Customer { get; set; }

        public DateTime SaleDate { get; set; }
        public SaleDocumentType DocumentType { get; set; }  // Pos = 1, Electronic = 2
        public SaleStatus Status { get; set; }  // Pending = 1, Completed = 2, Cancelled = 3

        // Totales (calculados desde los detalles)
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        // ========== CAMPOS EXCLUSIVOS PARA FACTURACIÓN ELECTRÓNICA ==========
        // (Estos campos serán NULL para facturas POS)

        // Numeración y control DIAN
        public string? ConsecutiveNumber { get; set; }      // Número de factura según resolución DIAN
        public string? CUFE { get; set; }                   // Código único de facturación electrónica
        public string? DianStatus { get; set; }             // Pendiente, Enviado, Aprobado, Rechazado
        public DateTime? DianSentDate { get; set; }         // Fecha de envío a DIAN
        public DateTime? DianApprovedDate { get; set; }     // Fecha de aprobación por DIAN
        public string? DianResponseMessage { get; set; }    // Mensaje de error si es rechazado

        // Datos de pago y comerciales
        public string? PaymentMethod { get; set; }          // Contado, Crédito, Tarjeta, etc.
        public DateTime? DueDate { get; set; }              // Fecha de vencimiento (si es crédito)
        public string? PurchaseOrderNumber { get; set; }    // Orden de compra del cliente

        // Archivos generados
        public string? XmlContent { get; set; }             // XML enviado a la DIAN
        public string? PdfUrl { get; set; }                 // URL del PDF generado

        // Relación con detalles
        public virtual ICollection<SaleDetailEntity> Details { get; set; } = new List<SaleDetailEntity>();
    }
}
