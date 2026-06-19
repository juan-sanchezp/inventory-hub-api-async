using AutoMapper;
using InventoryHub.DTOs.Sale;
using InventoryHub.Models;

namespace InventoryHub.Mapping
{
    public class PaymentMapper : Profile
    {
        public PaymentMapper()
        {
            CreateMap<PaymentEntity, PaymentResponseDTO>();
        }
    }
}
