using AutoMapper;
using InventoryHub.DTOs;
using InventoryHub.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InventoryHub.Mapping
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<ProductEntity, ProductDTO>();
            CreateMap<ProductDTO, ProductEntity>();
        }
    }
}