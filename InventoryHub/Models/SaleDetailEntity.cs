namespace InventoryHub.Models
{
    public class SaleDetailEntity
    {
        public int Id { get; set; }

        // Relación con la venta
        public int SaleId { get; set; }
        public virtual SaleEntity Sale { get; set; } = null!;

        // Relación con el producto
        public int ProductId { get; set; }
        public virtual ProductEntity Product { get; set; } = null!;

        // Datos de la transacción (precios congelados al momento de la venta)
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }      // Precio unitario al momento de la venta
        public decimal Discount { get; set; }       // Descuento aplicado a este detalle (opcional)
        public decimal TaxRate { get; set; }        // Tasa de IVA aplicada (0.19, 0.05, 0, etc.)

        // Totales calculados (opcionales pero útiles para reporting rápido)
        public decimal SubTotal { get; set; }       // Quantity * UnitPrice
        public decimal TaxAmount { get; set; }      // SubTotal * TaxRate
        public decimal Total { get; set; }          // SubTotal - Discount + TaxAmount
    }
}
