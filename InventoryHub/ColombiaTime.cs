namespace InventoryHub
{
    public static class ColombiaTime
    {
        private static readonly TimeZoneInfo ColombiaZone =
            TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        public static DateTime UtcToColombia(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ColombiaZone);
        }

        public static DateTime? UtcToColombia(DateTime? utcDateTime)
        {
            return utcDateTime.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, ColombiaZone)
                : null;
        }
    }
}
