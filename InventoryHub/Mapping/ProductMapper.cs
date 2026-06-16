using AutoMapper;
using InventoryHub.DTOs.Product;
using InventoryHub.Models;
using System.Linq;

namespace InventoryHub.Mapping
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            // Product -> DTO
            CreateMap<ProductEntity, ProductDTO>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.Name));

            // DTO -> Product
            CreateMap<ProductDTO, ProductEntity>()
                .ForMember(dest => dest.LedDetails,
                    opt => opt.Ignore());

            // Images
            CreateMap<ProductImageEntity, ProductImageDTO>()
                .ReverseMap();

            // LED details
            CreateMap<LedStripDetailsEntity, LedStripDetailsDTO>()
                .ForMember(dest => dest.CompatibleTVModels,
                    opt => opt.MapFrom(src =>
                        src.CompatibleTVs.Select(tv => tv.ModelCode)));

            CreateMap<LedStripDetailsDTO, LedStripDetailsEntity>()
                .ForMember(dest => dest.CompatibleTVs,
                    opt => opt.MapFrom(src =>
                        src.CompatibleTVModels != null
                            ? src.CompatibleTVModels.Select(code => new TVModelEntity
                            {
                                ModelCode = code
                            })
                            : new List<TVModelEntity>()));
        }
    }
}