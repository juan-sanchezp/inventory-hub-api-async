using AutoMapper;
using InventoryHub.DTOs.Sale;
using InventoryHub.Enums;
using InventoryHub.Models;
using InventoryHub.Repositories;

namespace InventoryHub.Services
{
    public class PaymentServiceImpl : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public PaymentServiceImpl(
            IPaymentRepository paymentRepository,
            ISaleRepository saleRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<List<PaymentResponseDTO>> GetBySaleId(int saleId)
        {
            var payments = await _paymentRepository.GetBySaleIdAsync(saleId);
            return _mapper.Map<List<PaymentResponseDTO>>(payments);
        }

        public async Task<PaymentResponseDTO?> RecordPayment(int saleId, RecordPaymentRequestDTO request)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null) return null;
            if (sale.Status == SaleStatus.Draft || sale.Status == SaleStatus.Cancelled) return null;

            var payment = new PaymentEntity
            {
                SaleId = saleId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                Reference = request.Reference,
                Notes = request.Notes,
                PaymentDate = DateTime.UtcNow
            };

            var saved = await _paymentRepository.AddAsync(payment);

            await RecalculatePaymentStatus(sale);

            return _mapper.Map<PaymentResponseDTO>(saved);
        }

        public async Task<List<PaymentResponseDTO>> GetByCustomerId(int customerId)
        {
            var payments = await _paymentRepository.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<PaymentResponseDTO>>(payments);
        }

        private async Task RecalculatePaymentStatus(SaleEntity sale)
        {
            var payments = await _paymentRepository.GetBySaleIdAsync(sale.Id);
            var totalPaid = payments.Sum(p => p.Amount);

            if (totalPaid <= 0)
                sale.PaymentStatus = PaymentStatus.Pending;
            else if (totalPaid < sale.Total)
                sale.PaymentStatus = PaymentStatus.Partial;
            else
                sale.PaymentStatus = PaymentStatus.Paid;

            await _saleRepository.UpdateAsync(sale);

            if (sale.CustomerId.HasValue)
            {
                await RecalculateCustomerBalance(sale.CustomerId.Value);
            }
        }

        private async Task RecalculateCustomerBalance(int customerId)
        {
            var customerSales = await _saleRepository.GetByCustomerIdAsync(customerId);
            var totalOwed = customerSales
                .Where(s => s.PaymentStatus == PaymentStatus.Pending
                         || s.PaymentStatus == PaymentStatus.Partial
                         || s.PaymentStatus == PaymentStatus.Overdue)
                .Sum(s => s.Total);

            var payments = await _paymentRepository.GetByCustomerIdAsync(customerId);
            var totalPaid = payments.Sum(p => p.Amount);

            var balance = totalOwed - totalPaid;
            if (balance < 0) balance = 0;

            await _saleRepository.UpdateCustomerBalance(customerId, balance);
        }
    }
}
