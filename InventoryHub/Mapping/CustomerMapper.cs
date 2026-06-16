using AutoMapper;
using InventoryHub.DTOs;
using InventoryHub.Models;

namespace InventoryHub.Mapping
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<CustomerEntity, CustomerDTO>()
                .ReverseMap();
        }
    }
}

