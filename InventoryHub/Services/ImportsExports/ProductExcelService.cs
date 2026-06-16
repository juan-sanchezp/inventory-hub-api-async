
    using ExcelDataReader;
    using global::InventoryHub.DTOs;
    using global::InventoryHub.Enums;
using InventoryHub.DTOs.Product;
using Microsoft.AspNetCore.Http;
    using OfficeOpenXml;
    using System.Data;

    namespace InventoryHub.Services.ImportsExports
    {
        public class ProductExcelService
        {
            public async Task<ImportResult> ImportProductsFullExcel(IFormFile file)
            {
                var result = new ImportResult
                {
                    Products = new List<ProductDTO>()
                };

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using var stream = file.OpenReadStream();
                using var reader = ExcelReaderFactory.CreateReader(stream);

                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                };

                var dataSet = reader.AsDataSet(conf);
                var table = dataSet.Tables[0];

                var existingCodes = new HashSet<string>();

                foreach (DataRow row in table.Rows)
                {
                    var product = MapRowToProductDTO(row, existingCodes, result);

                    if (product != null)
                    {
                        result.Products.Add(product);
                        result.Created++;
                    }
                    else
                    {
                        result.Duplicates++;
                    }
                }

                return result;
            }



            private ProductDTO? MapRowToProductDTO(DataRow row, HashSet<string> existingCodes, ImportResult result)
            {
                string code = row["code"]?.ToString()?.Trim() ?? "";

                if (string.IsNullOrEmpty(code) || existingCodes.Contains(code))
                    return null;

                existingCodes.Add(code);

                string categoryName = row["categoryName"]?.ToString()?.Trim() ?? "Sin categoría";

                var productDto = new ProductDTO
                {
                    Id = 0,
                    Code = code,
                    Barcode = row["barcode"]?.ToString() ?? "",
                    Name = row["name"]?.ToString() ?? "Sin nombre",
                    Brand = row["brand"]?.ToString() ?? "Sin marca",
                    Model = row["model"]?.ToString(),
                    Price = decimal.TryParse(row["price"]?.ToString(), out var price) ? price : 0,
                    //Stock = int.TryParse(row["stock"]?.ToString(), out var stock) ? stock : 0,
                    MinStock = int.TryParse(row["minStock"]?.ToString(), out var minStock) ? minStock : 0,
                    Description = row["description"]?.ToString(),
                    CategoryId = 0,
                    CategoryName = categoryName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Images = new List<ProductImageDTO>(),
                    LedDetails = MapLedDetails(row)
                };

                return productDto;
            }

            private LedStripDetailsDTO? MapLedDetails(DataRow row)
            {
                var inchStr = row["inch"]?.ToString();
                if (string.IsNullOrEmpty(inchStr))
                    return null;

                LedType ledTypeValue = LedType.Normal;

                var ledTypeStr = row["ledType"]?.ToString();
                if (!string.IsNullOrEmpty(ledTypeStr) && int.TryParse(ledTypeStr, out int ledTypeInt))
                {
                    if (Enum.IsDefined(typeof(LedType), ledTypeInt))
                        ledTypeValue = (LedType)ledTypeInt;
                }

                return new LedStripDetailsDTO
                {
                    Inch = int.TryParse(row["inch"]?.ToString(), out var inch) ? inch : 0,
                    StripCount = int.TryParse(row["stripCount"]?.ToString(), out var stripCount) ? stripCount : 0,
                    LengthMm = int.TryParse(row["lengthMm"]?.ToString(), out var lengthMm) ? lengthMm : null,
                    LedCount = int.TryParse(row["ledCount"]?.ToString(), out var ledCount) ? ledCount : null,
                    LedVolts = int.TryParse(row["ledVolts"]?.ToString(), out var ledVolts) ? ledVolts : null,
                    BoardCode = row["boardCode"]?.ToString(),
                    Distribution = row["distribution"]?.ToString(),
                    LedType = ledTypeValue,
                    Notes = row["notes"]?.ToString(),
                    CompatibleTVModels = string.IsNullOrEmpty(row["compatibleTVModels"]?.ToString())
                        ? new List<string>()
                        : row["compatibleTVModels"]?.ToString()
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList()
                };
            }

        public async Task<(byte[] FileContent, string FileName)> DownloadExcelTemplateAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Productos");

            string[] headers = new[]
            {
        "code","barcode","name","categoryName","brand","model",
        "price","minStock","description",
        "inch","stripCount","lengthMm","ledCount","ledVolts",
        "boardCode","distribution","ledType","notes","compatibleTVModels"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1].Value = headers[i];
                sheet.Cells[1, i + 1].Style.Font.Bold = true;
                sheet.Column(i + 1).AutoFit();
            }

            string[] exampleRow = new[]
            {
        "H65-3","7896541230987","Tiras LED Curvas LG","Tiras LED","HYLED","JS-D-JP65DM-C72FG",
        "120000","2","Tira LED para TV de 65 pulgadas",
        "65","12","800","8","6",
        "HY65-CB12","(3A+3B)","0","Cada tira tiene 8 LEDs","HYLED6518INTM,65DM1200"
    };

            for (int i = 0; i < exampleRow.Length; i++)
            {
                sheet.Cells[2, i + 1].Value = exampleRow[i];
            }

            sheet.Cells[1, 18].AddComment("Valores posibles: Normal=0, Cuadrado=1, SinLente=2", "Sistema");

            var bytes = package.GetAsByteArray();

            var fileName = $"Plantilla_Productos_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

            return (bytes, fileName);
        }

        public async Task<ImportResult> ImportStockProductsExcel(IFormFile file)
        {
            var result = new ImportResult
            {
                Products = new List<ProductDTO>()
            };

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var conf = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            };

            var dataSet = reader.AsDataSet(conf);
            var table = dataSet.Tables[0];

            var existingCodes = new HashSet<string>();

            foreach (DataRow row in table.Rows)
            {
                var product = MapRowToProductDTOStock(row, existingCodes, result);

                if (product != null)
                {
                    result.Products.Add(product);
                    result.Created++;
                }
                else
                {
                    result.Duplicates++;
                }
            }

            return result;
        }

        private ProductDTO? MapRowToProductDTOStock(
            DataRow row,
            HashSet<string> existingCodes,
            ImportResult result)
        {
            string code = row["code"]?.ToString()?.Trim().ToUpper() ?? "";

            if (string.IsNullOrEmpty(code) || existingCodes.Contains(code))
                return null;

            existingCodes.Add(code);

            int stock = 0;

            if (!int.TryParse(row["stock"]?.ToString(), out stock))
                stock = 0;

            var productDto = new ProductDTO
            {
                Code = code,
                Stock = stock,
                UpdatedAt = DateTime.UtcNow
            };

            return productDto;
        }




    }
}

