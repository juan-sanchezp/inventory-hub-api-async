using InventoryHub.DTOs;
using InventoryHub.Responses;
using InventoryHub.Services;
using InventoryHub.Services.CloudinaryS;
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
        public ProductController(IProductService service)
        {
            _service = service;
          
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
        public async Task<IActionResult> DeleteProductImage(
            int productId,
            [FromQuery] string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return BadRequest("Se requiere la URL de la imagen");

            var result = await _service.DeleteProductImage(productId, imageUrl);
            if (!result)
                return NotFound(ResponseFactory.Fail<string>("Imagen no encontrada"));

            return Ok(ResponseFactory.Success(imageUrl, "Imagen eliminada correctamente"));
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

    }

}