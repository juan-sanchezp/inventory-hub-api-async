using InventoryHub.DTOs;
using InventoryHub.Responses;
using InventoryHub.Services;
using InventoryHub.Services.ImportsExports;
using Microsoft.AspNetCore.Mvc;




namespace InventoryHub.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        //private readonly CloudinaryService _cloudinaryService;
        private readonly ProductExcelService _productExcelService;
        public ProductController(
            IProductService service,
            ProductExcelService productExcelService)
        {
            _service = service;
            _productExcelService = productExcelService;
        }


        // GET: api/products
        [HttpGet(Name = "GetAllProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _service.GetAll();
            return Ok(ResponseFactory.Success(products, "Products retrieved successfully"));
        }

        // GET: api/products/5
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _service.GetById(id);
            if (product == null)
                return NotFound(ResponseFactory.Fail<ProductDTO>("Product not found"));

            return Ok(ResponseFactory.Success(product, "Product retrieved successfully"));
        }

        // POST: api/products
        [HttpPost(Name = "SaveProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SaveProduct([FromBody] ProductDTO productDTO)
        {
            var product = await _service.Save(productDTO);
            if (product == null)
                return Conflict(ResponseFactory.Fail<ProductDTO>("Product already exists"));

            return CreatedAtRoute(
                "GetProductById",
                new { id = product.Id },
                ResponseFactory.Success(product, "Product created successfully")
            );
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDTO)
        {
            var updated = await _service.Update(id, productDTO);
            if (updated == null)
                return NotFound(ResponseFactory.Fail<ProductDTO>("Product not found"));

            return Ok(ResponseFactory.Success(updated, "Product updated successfully"));
        }

        [HttpPut("{id}/info")]
        public async Task<IActionResult> UpdateInfo(int id, UpdateProductInfoDTO dto)
        {
            var updated = await _service.UpdateInfo(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        // PATCH: api/products/stock
        [HttpPatch("stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockDTO dto)
        {
            if (dto == null)
                return BadRequest(ResponseFactory.Fail<UpdateStockDTO>("Invalid request body"));

            var updated = await _service.UpdateStockAsync(dto);

            if (updated == null)
                return NotFound(ResponseFactory.Fail<ProductDTO>("Product not found"));

            return Ok(ResponseFactory.Success(updated, "Stock updated successfully"));
        }


        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _service.DeleteById(id);
            if (deleted == null)
                return NotFound(ResponseFactory.Fail<ProductDTO>("Product not found"));

            return Ok(ResponseFactory.Success(deleted, "Product deleted successfully"));
        }

        //Filter
        [HttpGet("led-strips/search")]
        public async Task<IActionResult> SearchLedStrips([FromQuery] LedStripFilterDTO filter)
        {
            var results = await _service.SearchLedStrips(filter);
            return Ok(new { success = true, message = "Search completed", data = results });
        }


        //[HttpPost("{productId}/images")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadProductImages(
        //    int productId,
        //    [FromForm] IFormFile[] files)
        //{
        //    if (files == null || files.Length == 0)
        //        return BadRequest("No se enviaron archivos");

        //    var urls = new List<string>();

        //    foreach (var file in files)
        //    {
        //        var url = await _cloudinaryService.UploadImage(file);
        //        urls.Add(url);
        //    }

        //    return Ok(urls);
        //}


        [HttpPost("{productId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProductImages(
            int productId,
            [FromForm] IFormFile[] files)
        {
            if (files == null || files.Length == 0)
                return BadRequest("No se enviaron archivos");

            //var urls = new List<string>();
            var urls = await _service.UploadProductImages(productId, files);
            return Ok(urls);
        }

        // Reemplazar todas las imágenes de un producto
        [HttpPut("{productId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ReplaceProductImages(
            int productId,
            [FromForm] IFormFile[] files)
        {
            if (files == null || files.Length == 0)
                return BadRequest("No se enviaron archivos");

            var urls = await _service.ReplaceProductImages(productId, files);
            return Ok(ResponseFactory.Success(urls, "Imágenes reemplazadas correctamente"));
        }

        // Borrar una imagen específica por su ID o URL
        [HttpDelete("{productId}/images")]
        public async Task<IActionResult> DeleteProductImage(int productId,[FromQuery] string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return BadRequest("Se requiere la URL de la imagen");

            var result = await _service.DeleteProductImage(productId, publicId);
            if (!result)
                return NotFound(ResponseFactory.Fail<string>("Imagen no encontrada"));

            return Ok(ResponseFactory.Success("Imagen eliminada correctamente"));
        }


        [HttpPost("test-upload")]
        [Consumes("multipart/form-data")]
        public IActionResult TestImages([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No se enviaron archivos");
            }

            return Ok(new
            {
                count = files.Count
            });
        }


        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcelFull(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo Excel vacío");

            var result = await _service.ImportProductsFullExcel(file);

            return Ok(new
            {
                success = true,
                message = "Importación finalizada",
                created = result.Created,
                duplicates = result.Duplicates,
                result.Products
            });
        }

        [HttpPost("import-StockExcel")]
        public async Task<IActionResult> ImportStockExcell(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo Excel vacío");

            var result = await _service.ImportStockProductExcell(file);

            return Ok(new
            {
                success = true,
                message = "Importación finalizada",
                created = result.Created,
                duplicates = result.Duplicates,
                result.Products
            });
        }

        [HttpGet("download-excel-template")]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var (fileContent, fileName) = await _productExcelService.DownloadExcelTemplateAsync();

            return File(
                fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        //    [HttpGet("download-excel-template")]
        //    public IActionResult DownloadExcelTemplate()
        //    {

        //        using var package = new ExcelPackage();
        //        var sheet = package.Workbook.Worksheets.Add("Productos");

        //        // Encabezados
        //        string[] headers = new[]
        //        {
        //    "code", "barcode", "name", "categoryName", "brand", "model",
        //    "price", "minStock", "description",
        //    "inch", "stripCount", "lengthMm", "ledCount", "ledVolts",
        //    "boardCode", "distribution", "ledType", "notes", "compatibleTVModels"
        //};

        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            sheet.Cells[1, i + 1].Value = headers[i];
        //            sheet.Column(i + 1).AutoFit();
        //            sheet.Cells[1, i + 1].Style.Font.Bold = true;
        //        }

        //        // Ejemplo de fila
        //        string[] exampleRow = new[]
        //        {
        //    "H65-3", "7896541230987", "Tiras LED Curvas LG", "Tiras LED", "HYLED", "JS-D-JP65DM-C72FG",
        //    "120000", "2", "Tira LED para TV de 65 pulgadas",
        //    "65", "12", "800", "8", "6",
        //    "HY65-CB12", "(3A+3B)", "0", "Cada tira tiene 8 LEDs", "HYLED6518INTM,65DM1200,65XLEDPRO"
        //};

        //        for (int i = 0; i < exampleRow.Length; i++)
        //        {
        //            sheet.Cells[2, i + 1].Value = exampleRow[i];
        //        }

        //        // Comentario de LedType
        //        sheet.Cells[1, 18].AddComment("Valores posibles: Normal=0, Cuadrado=1, SinLente=2", "Sistema");

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);
        //        stream.Position = 0;

        //        string excelName = $"Plantilla_Productos_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

        //        return File(stream,
        //                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                    excelName);
        //    }

    }

}