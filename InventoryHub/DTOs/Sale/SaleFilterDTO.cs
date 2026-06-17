namespace InventoryHub.DTOs.Sale
{
    public class SaleFilterDTO
    {
        public string? ProductCode { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
