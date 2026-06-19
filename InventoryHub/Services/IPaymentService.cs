using InventoryHub.DTOs.Sale;

namespace InventoryHub.Services
{
    public interface IPaymentService
    {
        Task<List<PaymentResponseDTO>> GetBySaleId(int saleId);
        Task<PaymentResponseDTO?> RecordPayment(int saleId, RecordPaymentRequestDTO request);
        Task<List<PaymentResponseDTO>> GetByCustomerId(int customerId);
    }
}
