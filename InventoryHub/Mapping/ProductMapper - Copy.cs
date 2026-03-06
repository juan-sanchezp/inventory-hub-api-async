using InventoryHub.DTOs;
using InventoryHub.Models;

namespace InventoryHub.Mapping
{
    //mejor usar automaper

    // static class so it doesn't need to be instantiated
    public static class ProductMapperOld
    {
        // convert Entity to DTO
        public static ProductDTO ToDTO(ProductEntity entity)
        {
            if (entity == null) return null;

            return new ProductDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price
            };
        }

        // convert DTO to Entity
        public static ProductEntity ToEntity(ProductDTO dto)
        {
            if (dto == null) return null;

            return new ProductEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price
            };
        }

        public static List<ProductDTO> ToDTOList(List<ProductEntity> productsEntity)
        {
            if (productsEntity == null) return null;
            return productsEntity.Select(e => ToDTO(e)).ToList();
        }

        public static List<ProductEntity> ToEntityList(List<ProductDTO> productsDTO)
        {
            if (productsDTO == null) return null;
            return productsDTO.Select(d => ToEntity(d)).ToList();
        }

    }
}