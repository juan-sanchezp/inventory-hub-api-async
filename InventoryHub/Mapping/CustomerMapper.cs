using AutoMapper;
using InventoryHub.DTOs.Customer;
using InventoryHub.Models;

namespace InventoryHub.Mapping
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<CustomerEntity, CustomerDTO>();
            CreateMap<CustomerDTO, CustomerEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}

