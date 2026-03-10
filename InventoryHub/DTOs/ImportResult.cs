namespace InventoryHub.DTOs
{
    public class ImportResult
    {
        public int Created { get; set; }
        public int Duplicates { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
    }
}
