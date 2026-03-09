using AutoMapper;
using InventoryHub.Data;
using InventoryHub.DTOs;
using InventoryHub.Models;
using InventoryHub.Repositories;
using InventoryHub.Services.CloudinaryS;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;


        public ProductServiceImpl(IProductRepository productRepository, IMapper mapper, AppDbContext context)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _context = context;
        }

        // Obtener todos los productos con categorías y LedDetails
        public async Task<List<ProductDTO>> GetAll()
        {
            var productsEntity = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.LedDetails)
                    .ThenInclude(l => l.CompatibleTVs)
                .ToListAsync();

            return _mapper.Map<List<ProductDTO>>(productsEntity);
        }

        // Obtener producto por id
        public async Task<ProductDTO?> GetById(int id)
        {
            var productEntity = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.LedDetails)
                    .ThenInclude(l => l.CompatibleTVs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productEntity == null) return null;
            return _mapper.Map<ProductDTO>(productEntity);
        }

        // Guardar producto
        public async Task<ProductDTO?> Save(ProductDTO productDTO)
        {
            // Validar/crear categoría
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == productDTO.CategoryId);

            if (category == null)
            {
                // Puedes crear la categoría si no existe
                category = new Models.CategoryEntity { Name = productDTO.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            var entity = _mapper.Map<ProductEntity>(productDTO);
            entity.CategoryId = category.Id;

            // Manejar LedDetails si aplica
            if (productDTO.LedDetails != null)
            {
                var ledDetails = _mapper.Map<LedStripDetailsEntity>(productDTO.LedDetails);
                entity.LedDetails = ledDetails;
            }

            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(entity);
        }

        // Actualizar producto
        public async Task<ProductDTO?> Update(int id, ProductDTO productDTO)
        {
            var existingProduct = await _context.Products
                .Include(p => p.LedDetails)
                    .ThenInclude(l => l.CompatibleTVs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null) return null;

            // Actualiza propiedades básicas
            _mapper.Map(productDTO, existingProduct);

            // Actualiza LedDetails
            if (productDTO.LedDetails != null)
            {
                if (existingProduct.LedDetails == null)
                {
                    existingProduct.LedDetails = _mapper.Map<LedStripDetailsEntity>(productDTO.LedDetails);
                }
                else
                {
                    // Actualiza campos simples
                    existingProduct.LedDetails.LengthMm = productDTO.LedDetails.LengthMm;
                    existingProduct.LedDetails.LedCount = productDTO.LedDetails.LedCount;

                    // Reemplaza modelos de TV
                    existingProduct.LedDetails.CompatibleTVs.Clear();
                    if (productDTO.LedDetails.CompatibleTVModels != null)
                    {
                        foreach (var code in productDTO.LedDetails.CompatibleTVModels)
                        {
                            existingProduct.LedDetails.CompatibleTVs.Add(new TVModelEntity { ModelCode = code });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDTO>(existingProduct);
        }

        // Eliminar producto
        public async Task<ProductDTO?> DeleteById(int id)
        {
            var productEntity = await _context.Products
                .Include(p => p.LedDetails)
                    .ThenInclude(l => l.CompatibleTVs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productEntity == null) return null;

            _context.Products.Remove(productEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task<List<ProductDTO>> SearchLedStrips(LedStripFilterDTO dto)
        {
            var filter = new LedStripFilter
            {
                CompatibleTVModel = dto.CompatibleTVModel,
                MinLedCount = dto.MinLedCount,
                MaxLedCount = dto.MaxLedCount,
                MinLengthMm = dto.MinLengthMm,
                MaxLengthMm = dto.MaxLengthMm,
                LedVolts = dto.LedVolts
            };

            var entities = await _productRepository.SearchLedStripsAsync(filter);
            return _mapper.Map<List<ProductDTO>>(entities);
        }

        //public async Task<List<string>> UploadProductImages(int productId, IFormFile[] files)
        //{
        //    if (files == null || files.Length == 0)
        //        throw new Exception("No files received");

        //    var urls = new List<string>();

        //    foreach (var file in files)
        //    {
        //        var url = await _cloudinaryService.UploadImage(file);
        //        urls.Add(url);
        //    }

        //    return urls;
        //}

    }


 }