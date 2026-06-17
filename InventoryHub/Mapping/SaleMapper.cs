using AutoMapper;
using InventoryHub.DTOs.Sale;
using InventoryHub.Models;

namespace InventoryHub.Mapping
{
    public class SaleMapper : Profile
    {
        public SaleMapper()
        {
            // ==================== SALE MAPPINGS ====================

            // Entity -> Response DTO
            CreateMap<SaleEntity, SaleResponseDTO>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null))
                .ForMember(dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details));

            // Create Request -> Entity
            CreateMap<CreateSaleRequestDTO, SaleEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.SaleDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.Tax, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore())
                .ForMember(dest => dest.ConsecutiveNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CUFE, opt => opt.Ignore())
                .ForMember(dest => dest.DianStatus, opt => opt.Ignore())
                .ForMember(dest => dest.DianSentDate, opt => opt.Ignore())
                .ForMember(dest => dest.DianApprovedDate, opt => opt.Ignore())
                .ForMember(dest => dest.DianResponseMessage, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.XmlContent, opt => opt.Ignore())
                .ForMember(dest => dest.PdfUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.Ignore());

            // Update Request -> Entity
            CreateMap<UpdateSaleRequestDTO, SaleEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.SaleDate, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.Tax, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore())
                .ForMember(dest => dest.DianSentDate, opt => opt.Ignore())
                .ForMember(dest => dest.DianApprovedDate, opt => opt.Ignore())
                .ForMember(dest => dest.DianResponseMessage, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.XmlContent, opt => opt.Ignore())
                .ForMember(dest => dest.PdfUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.Ignore());

            // Checkout Request -> Entity (solo actualiza campos específicos)
            CreateMap<CheckoutRequestDTO, SaleEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.SaleDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.Tax, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore())
                .ForMember(dest => dest.ConsecutiveNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CUFE, opt => opt.Ignore())
                .ForMember(dest => dest.DianStatus, opt => opt.Ignore())
                .ForMember(dest => dest.DianSentDate, opt => opt.Ignore())
                .ForMember(dest => dest.DianApprovedDate, opt => opt.Ignore())
                .ForMember(dest => dest.DianResponseMessage, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.XmlContent, opt => opt.Ignore())
                .ForMember(dest => dest.PdfUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.Ignore());

            // ==================== SALE DETAIL MAPPINGS ====================

            // Entity -> Response DTO
            CreateMap<SaleDetailEntity, SaleDetailResponseDTO>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));

            // Add to Cart Request -> Entity
            CreateMap<AddToCartRequestDTO, SaleDetailEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SaleId, opt => opt.Ignore())
                .ForMember(dest => dest.Sale, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.TaxRate, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.TaxAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore());

            // Update Cart Item Request -> Entity
            CreateMap<UpdateCartItemRequestDTO, SaleDetailEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SaleId, opt => opt.Ignore())
                .ForMember(dest => dest.Sale, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.TaxRate, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.TaxAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore());

            // ==================== REVERSE MAPPINGS (si son necesarios) ====================

            // Response DTO -> Entity (para casos específicos)
            CreateMap<SaleResponseDTO, SaleEntity>()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<SaleDetailResponseDTO, SaleDetailEntity>()
                .ForMember(dest => dest.Sale, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}