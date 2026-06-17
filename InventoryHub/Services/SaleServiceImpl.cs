using AutoMapper;
using InventoryHub.DTOs.Sale;
using InventoryHub.Enums;
using InventoryHub.Models;
using InventoryHub.Repositories;


namespace InventoryHub.Services
{
    public class SaleServiceImpl : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public SaleServiceImpl(
            ISaleRepository saleRepository,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        // ==================== BASIC CRUD ====================

        public async Task<List<SaleResponseDTO>> GetAll()
        {
            var salesEntity = await _saleRepository.GetAllAsync();
            return _mapper.Map<List<SaleResponseDTO>>(salesEntity);
        }

        public async Task<SaleResponseDTO?> GetById(int id)
        {
            var saleEntity = await _saleRepository.GetByIdAsync(id);
            if (saleEntity == null) return null;
            return _mapper.Map<SaleResponseDTO>(saleEntity);
        }

        public async Task<SaleResponseDTO?> Save(CreateSaleRequestDTO saleDTO)
        {
            var saleEntity = _mapper.Map<SaleEntity>(saleDTO);

            // Valores por defecto
            saleEntity.SaleDate = DateTime.UtcNow;
            saleEntity.Status = SaleStatus.Draft;  // Empieza como borrador/carrito
            saleEntity.SubTotal = 0;
            saleEntity.Tax = 0;
            saleEntity.Discount = 0;
            saleEntity.Total = 0;

            var savedEntity = await _saleRepository.AddAsync(saleEntity);
            if (savedEntity == null) return null;

            return _mapper.Map<SaleResponseDTO>(savedEntity);
        }

        public async Task<SaleResponseDTO?> Update(int id, UpdateSaleRequestDTO saleDTO)
        {
            var saleEntity = await _saleRepository.GetByIdAsync(id);
            if (saleEntity == null) return null;

            _mapper.Map(saleDTO, saleEntity);

            var updatedEntity = await _saleRepository.UpdateAsync(saleEntity);
            return _mapper.Map<SaleResponseDTO>(updatedEntity);
        }

        public async Task<SaleResponseDTO?> DeleteById(int id)
        {
            var saleEntity = await _saleRepository.GetByIdAsync(id);
            if (saleEntity == null) return null;

            bool deleted = await _saleRepository.DeleteAsync(saleEntity);
            if (!deleted) return null;

            return _mapper.Map<SaleResponseDTO>(saleEntity);
        }

        // ==================== CART OPERATIONS ====================

        public async Task<SaleResponseDTO?> GetCart()
        {
            var cartEntity = await _saleRepository.GetCartAsync();
            if (cartEntity == null)
            {
                // Crear un nuevo carrito si no existe
                var newCart = new SaleEntity
                {
                    SaleDate = DateTime.UtcNow,
                    Status = SaleStatus.Draft,
                    DocumentType = SaleDocumentType.Pos,
                    SubTotal = 0,
                    Tax = 0,
                    Discount = 0,
                    Total = 0,
                    Details = new List<SaleDetailEntity>()
                };

                var savedCart = await _saleRepository.AddAsync(newCart);
                return _mapper.Map<SaleResponseDTO>(savedCart);
            }

            return _mapper.Map<SaleResponseDTO>(cartEntity);
        }

        public async Task<SaleResponseDTO?> AddToCart(AddToCartRequestDTO request)
        {
            var cart = await _saleRepository.GetCartAsync();
            if (cart == null)
            {
                cart = new SaleEntity
                {
                    SaleDate = DateTime.UtcNow,
                    Status = SaleStatus.Draft,
                    DocumentType = SaleDocumentType.Pos,
                    SubTotal = 0,
                    Tax = 0,
                    Discount = 0,
                    Total = 0,
                    Details = new List<SaleDetailEntity>()
                };
                cart = await _saleRepository.AddAsync(cart);
            }

            // Verificar si el producto ya existe en el carrito
            var existingDetail = cart.Details.FirstOrDefault(d => d.ProductId == request.ProductId);

            if (existingDetail != null)
            {
                // Actualizar cantidad
                existingDetail.Quantity += request.Quantity;
                await _saleRepository.UpdateDetailAsync(existingDetail);
            }
            else
            {
                // Obtener el producto para sus datos
                var product = await _saleRepository.GetProductByIdAsync(request.ProductId);
                if (product == null) return null;

                var taxRate = product.TaxRate ?? 0m;

                var subtotal = product.Price * request.Quantity;
                var taxAmount = subtotal * taxRate;
                var newDetail = new SaleDetailEntity
                {
                    SaleId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price,
                    Discount = 0,
                    TaxRate = taxRate,
                    SubTotal = subtotal,
                    TaxAmount = taxAmount,
                    Total = subtotal - 0 + taxAmount
                };

                await _saleRepository.AddDetailAsync(newDetail);
            }

            // Recalcular totales del carrito
            await RecalculateSaleTotals(cart.Id);

            var updatedCart = await _saleRepository.GetByIdAsync(cart.Id);
            return _mapper.Map<SaleResponseDTO>(updatedCart);
        }

        public async Task<SaleResponseDTO?> UpdateCartItem(int detailId, UpdateCartItemRequestDTO request)
        {
            var detail = await _saleRepository.GetDetailByIdAsync(detailId);
            if (detail == null) return null;

            var sale = await _saleRepository.GetByIdAsync(detail.SaleId);
            if (sale == null || sale.Status != SaleStatus.Draft) return null;

            // Actualizar cantidad
            detail.Quantity = request.Quantity;
            detail.SubTotal = detail.UnitPrice * detail.Quantity;
            detail.TaxAmount = detail.SubTotal * detail.TaxRate;
            detail.Total = detail.SubTotal - detail.Discount + detail.TaxAmount;

            await _saleRepository.UpdateDetailAsync(detail);

            // Recalcular totales
            await RecalculateSaleTotals(sale.Id);

            var updatedSale = await _saleRepository.GetByIdAsync(sale.Id);
            return _mapper.Map<SaleResponseDTO>(updatedSale);
        }

        public async Task<SaleResponseDTO?> RemoveFromCart(int detailId)
        {
            var detail = await _saleRepository.GetDetailByIdAsync(detailId);
            if (detail == null) return null;

            var saleId = detail.SaleId;
            await _saleRepository.DeleteDetailAsync(detail);

            // Recalcular totales
            await RecalculateSaleTotals(saleId);

            var updatedSale = await _saleRepository.GetByIdAsync(saleId);
            return _mapper.Map<SaleResponseDTO>(updatedSale);
        }

        public async Task<SaleResponseDTO?> ClearCart()
        {
            var cart = await _saleRepository.GetCartAsync();
            if (cart == null) return null;

            await _saleRepository.ClearDetailsAsync(cart.Id);

            // Resetear totales
            cart.SubTotal = 0;
            cart.Tax = 0;
            cart.Discount = 0;
            cart.Total = 0;
            await _saleRepository.UpdateAsync(cart);

            return _mapper.Map<SaleResponseDTO>(cart);
        }

        public async Task<SaleResponseDTO?> Checkout(int saleId, CheckoutRequestDTO request)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null) return null;
            if (sale.Status != SaleStatus.Draft) return null;
            if (sale.Details == null || !sale.Details.Any()) return null;

            // Actualizar datos de la venta
            sale.Status = SaleStatus.Completed;
            sale.CustomerId = request.CustomerId;
            sale.DocumentType = request.DocumentType;

            if (request.DocumentType == SaleDocumentType.Electronic)
            {
                sale.PaymentMethod = request.PaymentMethod;
                sale.DueDate = request.DueDate;
                sale.DianStatus = "Pendiente";
            }

            await _saleRepository.UpdateAsync(sale);

            // Descontar inventario
            foreach (var detail in sale.Details)
            {
                await _saleRepository.UpdateProductStock(detail.ProductId, detail.Quantity);
            }

            return _mapper.Map<SaleResponseDTO>(sale);
        }

        // ==================== SALE DETAILS MANAGEMENT ====================

        public async Task<SaleDetailResponseDTO?> GetSaleDetailById(int detailId)
        {
            var detail = await _saleRepository.GetDetailByIdAsync(detailId);
            if (detail == null) return null;
            return _mapper.Map<SaleDetailResponseDTO>(detail);
        }

        public async Task<List<SaleDetailResponseDTO>> GetSaleDetailsBySaleId(int saleId)
        {
            var details = await _saleRepository.GetDetailsBySaleIdAsync(saleId);
            return _mapper.Map<List<SaleDetailResponseDTO>>(details);
        }

        // ==================== ELECTRONIC INVOICE ====================

        public async Task<SaleResponseDTO?> SendToDian(int saleId)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null) return null;
            if (sale.DocumentType != SaleDocumentType.Electronic) return null;
            if (sale.Status != SaleStatus.Completed) return null;

            // Aquí iría la lógica de envío a DIAN
            // Por ahora solo simulamos
            sale.DianStatus = "Enviado";
            sale.DianSentDate = DateTime.UtcNow;

            await _saleRepository.UpdateAsync(sale);

            return _mapper.Map<SaleResponseDTO>(sale);
        }

        // ==================== PRIVATE HELPERS ====================

        private async Task RecalculateSaleTotals(int saleId)
        {
            var details = await _saleRepository.GetDetailsBySaleIdAsync(saleId);
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null) return;

            sale.SubTotal = details.Sum(d => d.SubTotal);
            sale.Tax = details.Sum(d => d.TaxAmount);
            sale.Total = details.Sum(d => d.Total);

            await _saleRepository.UpdateAsync(sale);
        }
    }
}