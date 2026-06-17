using InventoryHub.DTOs.Sale;

namespace InventoryHub.Services
{
    public interface ISaleService
    {
        // Basic CRUD operations
        Task<List<SaleResponseDTO>> GetAll();
        Task<SaleResponseDTO?> GetById(int id);
        Task<SaleResponseDTO?> Save(CreateSaleRequestDTO saleDTO);
        Task<SaleResponseDTO?> Update(int id, UpdateSaleRequestDTO saleDTO);
        Task<SaleResponseDTO?> DeleteById(int id);

        // Cart operations (carrito de compras)
        Task<SaleResponseDTO?> GetCart();  // Obtener carrito activo (Status = Draft)
        Task<SaleResponseDTO?> AddToCart(AddToCartRequestDTO request);
        Task<SaleResponseDTO?> UpdateCartItem(int detailId, UpdateCartItemRequestDTO request);
        Task<SaleResponseDTO?> RemoveFromCart(int detailId);
        Task<SaleResponseDTO?> ClearCart();
        Task<SaleResponseDTO?> Checkout(int saleId, CheckoutRequestDTO request);  // Confirmar venta

        // Sale details management (desde SaleService)
        Task<SaleDetailResponseDTO?> GetSaleDetailById(int detailId);
        Task<List<SaleDetailResponseDTO>> GetSaleDetailsBySaleId(int saleId);

        // Electronic invoice methods (para futuro)
        Task<SaleResponseDTO?> SendToDian(int saleId);
    }
}