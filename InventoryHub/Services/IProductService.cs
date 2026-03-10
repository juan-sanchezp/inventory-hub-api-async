
using InventoryHub.DTOs;
using System.Threading.Tasks;

namespace InventoryHub.Services
{
    public interface IProductService
    {

        Task<List<ProductDTO>> GetAll();
        Task<ProductDTO> GetById(int id);
        Task<ProductDTO> Save(ProductDTO productDTO);

        Task<ProductDTO> Update(int id, ProductDTO productDTO);
        Task<ProductDTO> UpdateStockAsync(UpdateStockDTO dto);

        Task<ProductDTO> DeleteById(int id);

        Task<List<ProductDTO>> SearchLedStrips(LedStripFilterDTO filter);

        Task<List<string>> UploadProductImages(int productId, IFormFile[] files);
        Task<List<string>> ReplaceProductImages(int productId, IFormFile[] files);
        Task<bool> DeleteProductImage(int productId, string imageUrl);
    }
}
