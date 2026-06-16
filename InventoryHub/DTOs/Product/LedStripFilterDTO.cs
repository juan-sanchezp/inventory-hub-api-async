namespace InventoryHub.DTOs.Product
{
        public class LedStripFilterDTO
        {
            // Búsqueda general dinámica
            public string? Search { get; set; }
        // Busca en Código / Nombre, barcode

        public string? CompatibleTVModel { get; set; }  //busqueda por modelo de tv 

            public int? Inch { get; set; }

            public int? StripCount { get; set; }

            public int? LedVolts { get; set; }

            public int? LedCount { get; set; }
            public int? LedType { get; set; }

            public string? BoardCode { get; set; }
      }
    }
