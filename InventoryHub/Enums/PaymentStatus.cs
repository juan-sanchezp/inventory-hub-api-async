namespace InventoryHub.Enums
{
    /// <summary>
    /// Representa el estado financiero de una venta.
    /// Se determina según el monto pagado y el saldo pendiente.
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// No se ha registrado ningún pago.
        /// El cliente debe el valor total de la venta.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Se ha realizado uno o más pagos, pero aún existe un saldo pendiente.
        /// </summary>
        Partial = 2,

        /// <summary>
        /// La venta ha sido pagada en su totalidad.
        /// No existe deuda pendiente.
        /// </summary>
        Paid = 3,

        /// <summary>
        /// La fecha límite de pago fue superada y aún existe un saldo pendiente.
        /// Solo aplica para ventas a crédito o fiadas.
        /// </summary>
        Overdue = 4,

        /// <summary>
        /// La venta o la obligación de pago fue cancelada.
        /// Generalmente se utiliza cuando la venta es anulada o revertida.
        /// </summary>
        Cancelled = 5
    }
}