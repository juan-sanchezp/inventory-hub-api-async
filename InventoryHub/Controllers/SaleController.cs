using InventoryHub.DTOs;
using InventoryHub.Responses;
using InventoryHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // ==================== BASIC CRUD ====================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _saleService.GetAll();
            return Ok(ResponseFactory.Success(sales, "Sales retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetById(id);

            if (sale == null)
                return NotFound(ResponseFactory.Fail<object>($"Sale with id {id} not found"));

            return Ok(ResponseFactory.Success(sale, "Sale retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CreateSaleRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseFactory.Fail<object>("Invalid data"));

            var sale = await _saleService.Save(request);

            if (sale == null)
                return BadRequest(ResponseFactory.Fail<object>("Could not create sale"));

            return Ok(ResponseFactory.Success(sale, "Sale created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSaleRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseFactory.Fail<object>("Invalid data"));

            var sale = await _saleService.Update(id, request);

            if (sale == null)
                return NotFound(ResponseFactory.Fail<object>($"Sale with id {id} not found"));

            return Ok(ResponseFactory.Success(sale, "Sale updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var sale = await _saleService.DeleteById(id);

            if (sale == null)
                return NotFound(ResponseFactory.Fail<object>($"Sale with id {id} not found"));

            return Ok(ResponseFactory.Success(sale, "Sale deleted successfully"));
        }

        // ==================== CART ====================

        [HttpGet("cart")]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _saleService.GetCart();
            return Ok(ResponseFactory.Success(cart, "Cart retrieved successfully"));
        }

        [HttpPost("cart/add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseFactory.Fail<object>("Invalid data"));

            var cart = await _saleService.AddToCart(request);

            if (cart == null)
                return BadRequest(ResponseFactory.Fail<object>("Could not add product to cart"));

            return Ok(ResponseFactory.Success(cart, "Product added to cart successfully"));
        }

        [HttpPut("cart/item/{detailId}")]
        public async Task<IActionResult> UpdateCartItem(int detailId, [FromBody] UpdateCartItemRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseFactory.Fail<object>("Invalid data"));

            var cart = await _saleService.UpdateCartItem(detailId, request);

            if (cart == null)
                return NotFound(ResponseFactory.Fail<object>($"Cart item with id {detailId} not found"));

            return Ok(ResponseFactory.Success(cart, "Cart item updated successfully"));
        }

        [HttpDelete("cart/item/{detailId}")]
        public async Task<IActionResult> RemoveFromCart(int detailId)
        {
            var cart = await _saleService.RemoveFromCart(detailId);

            if (cart == null)
                return NotFound(ResponseFactory.Fail<object>($"Cart item with id {detailId} not found"));

            return Ok(ResponseFactory.Success(cart, "Item removed from cart successfully"));
        }

        [HttpDelete("cart/clear")]
        public async Task<IActionResult> ClearCart()
        {
            var cart = await _saleService.ClearCart();

            if (cart == null)
                return NotFound(ResponseFactory.Fail<object>("No active cart found"));

            return Ok(ResponseFactory.Success(cart, "Cart cleared successfully"));
        }

        [HttpPost("{saleId}/checkout")]
        public async Task<IActionResult> Checkout(int saleId, [FromBody] CheckoutRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseFactory.Fail<object>("Invalid data"));

            var sale = await _saleService.Checkout(saleId, request);

            if (sale == null)
                return BadRequest(ResponseFactory.Fail<object>("Could not complete checkout"));

            return Ok(ResponseFactory.Success(sale, "Checkout completed successfully"));
        }

        // ==================== DETAILS ====================

        [HttpGet("{saleId}/details")]
        public async Task<IActionResult> GetSaleDetails(int saleId)
        {
            var details = await _saleService.GetSaleDetailsBySaleId(saleId);
            return Ok(ResponseFactory.Success(details, "Sale details retrieved successfully"));
        }

        [HttpGet("details/{detailId}")]
        public async Task<IActionResult> GetSaleDetailById(int detailId)
        {
            var detail = await _saleService.GetSaleDetailById(detailId);

            if (detail == null)
                return NotFound(ResponseFactory.Fail<object>($"Sale detail with id {detailId} not found"));

            return Ok(ResponseFactory.Success(detail, "Sale detail retrieved successfully"));
        }

        // ==================== DIAN ====================

        [HttpPost("{saleId}/send-to-dian")]
        public async Task<IActionResult> SendToDian(int saleId)
        {
            var sale = await _saleService.SendToDian(saleId);

            if (sale == null)
                return BadRequest(ResponseFactory.Fail<object>(
                    "Could not send invoice to DIAN"));

            return Ok(ResponseFactory.Success(sale, "Invoice sent to DIAN successfully"));
        }
    }
}