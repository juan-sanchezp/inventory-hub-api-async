namespace InventoryHub.Models
{
    public class LedStripFilter
    {
        // Búsqueda general dinámica
        public string? Search { get; set; }
        // Busca en Name, Code, Barcode, Model

        public string? CompatibleTVModel { get; set; }

        public int? Inch { get; set; }

        public int? StripCount { get; set; }

        public int? LedVolts { get; set; }

        public int? LedCount { get; set; }

        public string? BoardCode { get; set; }
    }
}
