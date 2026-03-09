namespace InventoryHub.Models
{
    public class LedStripFilter
    {
        public string? CompatibleTVModel { get; set; }
        public int? MinLedCount { get; set; }
        public int? MaxLedCount { get; set; }
        public int? MinLengthMm { get; set; }
        public int? MaxLengthMm { get; set; }
        public string? LedVolts { get; set; }
    }
}
