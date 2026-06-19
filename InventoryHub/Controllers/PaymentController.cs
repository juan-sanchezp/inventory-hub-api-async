using InventoryHub.DTOs.Sale;
using InventoryHub.Responses;
using InventoryHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("sale/{saleId}")]
        public async Task<IActionResult> GetBySaleId(int saleId)
        {
            var payments = await _paymentService.GetBySaleId(saleId);
            return Ok(ResponseFactory.Success(payments, "Payments retrieved successfully"));
        }

        [HttpPost("sale/{saleId}")]
        public async Task<IActionResult> RecordPayment(int saleId, [FromBody] RecordPaymentRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseFactory.Fail<object>("Invalid data"));

            var payment = await _paymentService.RecordPayment(saleId, request);

            if (payment == null)
                return BadRequest(ResponseFactory.Fail<object>("Could not record payment. Sale may not exist or is not active."));

            return Ok(ResponseFactory.Success(payment, "Payment recorded successfully"));
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var payments = await _paymentService.GetByCustomerId(customerId);
            return Ok(ResponseFactory.Success(payments, "Payments retrieved successfully"));
        }
    }
}
