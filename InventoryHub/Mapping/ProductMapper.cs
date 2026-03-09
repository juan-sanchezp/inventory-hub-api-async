using AutoMapper;
using InventoryHub.DTOs;
using InventoryHub.Models;
using System.Linq;

namespace InventoryHub.Mapping
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            // Product <-> ProductDTO
            CreateMap<ProductEntity, ProductDTO>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.LedDetails,
                           opt => opt.MapFrom(src => src.LedDetails != null ? src.LedDetails : null));

            CreateMap<ProductDTO, ProductEntity>()
                .ForMember(dest => dest.LedDetails,
                           opt => opt.Ignore()); // Lo manejas aparte en repo

            // LedDetails <-> LedDetailsDTO
            CreateMap<LedStripDetailsEntity, LedStripDetailsDTO>()
                .ForMember(dest => dest.CompatibleTVModels,
                           opt => opt.MapFrom(src => src.CompatibleTVs.Select(tv => tv.ModelCode).ToList()));

            CreateMap<LedStripDetailsDTO, LedStripDetailsEntity>()
                .ForMember(dest => dest.CompatibleTVs,
                           opt => opt.MapFrom(src => src.CompatibleTVModels != null
                               ? src.CompatibleTVModels.Select(code => new TVModelEntity { ModelCode = code }).ToList()
                               : new List<TVModelEntity>()));
        }
    }
}