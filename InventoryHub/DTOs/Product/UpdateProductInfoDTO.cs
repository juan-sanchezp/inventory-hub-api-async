namespace InventoryHub.DTOs.Product
{
    public class UpdateProductInfoDTO
    {
        public string Name { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        //no estaba
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public int MinStock { get; set; }

        public int CantOrderStock { get; set; }

     


        public string Description { get; set; }

        public bool IsActive { get; set; }

        public LedDetailsUpdateDTO LedDetails { get; set; }
    }

    public class LedDetailsUpdateDTO
    {
        public int Inch { get; set; }

        public int StripCount { get; set; }

        public int LengthMm { get; set; }

        public int LedCount { get; set; }

        public int LedVolts { get; set; }
        public int LedType { get; set; }

        public string BoardCode { get; set; }

        public string Distribution { get; set; }

        public string Notes { get; set; }

        public List<string> CompatibleTVModels { get; set; }
    }
}
