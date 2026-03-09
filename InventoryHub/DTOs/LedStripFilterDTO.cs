namespace InventoryHub.DTOs
{
    public class LedStripFilterDTO
    {
        public string? CompatibleTVModel { get; set; }  // Búsqueda por modelo de TV
        public int? MinLedCount { get; set; }
        public int? MaxLedCount { get; set; }
        public int? MinLengthMm { get; set; }
        public int? MaxLengthMm { get; set; }
        public string? LedVolts { get; set; } // Ej: "6V", "3V"
    }
}
