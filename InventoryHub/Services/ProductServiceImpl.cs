using AutoMapper;
using InventoryHub.Data;
using InventoryHub.DTOs.Product;
using InventoryHub.Enums;
using InventoryHub.Models;
using InventoryHub.Repositories;
using InventoryHub.Services.CloudinaryS;
using InventoryHub.Services.ImportsExports;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace InventoryHub.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _productAccessBd;
        private readonly ICategoryRepository _categoryAccessBd;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ProductExcelService _productExcelService;

        public ProductServiceImpl(IProductRepository productRepository, ICategoryRepository categoryAccessBd, 
            IMapper mapper, AppDbContext context, CloudinaryService cloudinaryService, ProductExcelService productExcelService)
        {
            _productAccessBd = productRepository;
            _categoryAccessBd = categoryAccessBd;
            _mapper = mapper;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _productExcelService = productExcelService;
        }

        // Obtener todos los productos con categorías y LedDetails
        public async Task<List<ProductDTO>> GetAll()
        {
            var productsEntity = await _productAccessBd.GetAllAsync();
            return _mapper.Map<List<ProductDTO>>(productsEntity);
        }

        public async Task<ProductDTO?> GetById(int id)
        {
            var productEntity = await _productAccessBd.GetByIdAsync(id);
            if (productEntity == null) return null;
            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task<ProductDTO?> Save(ProductDTO productDTO)
        {
            // Buscar o crear categoría
            var category = await _categoryAccessBd.GetByIdAsync(productDTO.CategoryId);
            if (category == null)
            {
                category = new CategoryEntity { Name = productDTO.CategoryName };
                category = await _categoryAccessBd.AddAsync(category);
            }
            productDTO.Code = productDTO.Code.Trim().ToUpper(); //normalizar code

            // Mapear DTO a Entity
            var entity = _mapper.Map<ProductEntity>(productDTO);
            entity.CategoryId = category.Id;

            // Mapear LedDetails si existen
            if (productDTO.LedDetails != null)
            {
                entity.LedDetails = _mapper.Map<LedStripDetailsEntity>(productDTO.LedDetails);
            }

            // Guardar usando ProductRepository
            var savedEntity = await _productAccessBd.AddAsync(entity);
            if (savedEntity == null) return null; // ya existía

            // Retornar DTO
            return _mapper.Map<ProductDTO>(savedEntity);
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

        public async Task<ProductDTO?> UpdateStockAsync(UpdateStockDTO dto)
        {
            var product = await _productAccessBd.GetByIdAsync(dto.Id);

            if (product == null) return null;

            // Validación opcional
            if (dto.Stock < 0) throw new ArgumentException("Stock cannot be negative");

            product.Stock = dto.Stock;
            product.UpdatedAt = DateTime.UtcNow;
            ProductEntity productEntity = await _productAccessBd.UpdateAsync(product);
            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task<ProductDTO?> DeleteById(int id)
        {
            var productEntity = await _productAccessBd.GetByIdAsync(id);

            if (productEntity == null) return null;
            if(await _productAccessBd.DeleteAsync(productEntity) == false)
                { return null; }
   
            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task<List<ProductDTO>> SearchLedStrips(LedStripFilterDTO dto)
        {
            var filter = new LedStripFilter
            {
                Search = dto.Search,
                CompatibleTVModel = dto.CompatibleTVModel,
                Inch = dto.Inch,
                StripCount = dto.StripCount,
                LedVolts = dto.LedVolts,
                LedCount = dto.LedCount,
                LedType = dto.LedType,
                BoardCode = dto.BoardCode
            };

            var entities = await _productAccessBd.SearchLedStripsAsync(filter);

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
        public async Task<List<string>> UploadProductImages(int productId, IFormFile[] files)
        {
            var product = await _productAccessBd.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Producto no encontrado");

            var urls = new List<string>();
            int counter = 1; // contador dentro del mismo lote

            foreach (var file in files)
            {
                string datePart = DateTime.UtcNow.ToString("yyMMdds");
                string publicId = $"{product.Code}_{datePart}_{counter}";

                // Subir a Cloudinary
                var result = await _cloudinaryService.UploadImage(file, publicId);

                // Crear entidad imagen
                var image = new ProductImageEntity
                {
                    Url = result.Url,
                    PublicId = result.PublicId,
                    ProductId = productId,
                    IsMain = !product.Images.Any() // la primera imagen principal
                };

                product.Images.Add(image);
                urls.Add(result.Url);

                counter++; // siguiente imagen en el lote
            }

            product.UpdatedAt = DateTime.UtcNow;
            await _productAccessBd.UpdateAsync(product);

            return urls;
        }
        //public async Task<List<string>> UploadProductImages(int productId, IFormFile[] files)
        //{
        //    var product = await _productAccessBd.GetByIdAsync(productId);

        //    if (product == null)
        //        throw new Exception("Producto no encontrado");

        //    var urls = new List<string>();

        //    foreach (var file in files)
        //    {
        //        // Subir a Cloudinary
        //        var result = await _cloudinaryService.UploadImage(file);

        //        // Crear entidad imagen
        //        var image = new ProductImageEntity
        //        {
        //            Url = result.Url,
        //            PublicId = result.PublicId,
        //            ProductId = productId,
        //            IsMain = product.Images.Count == 0 // la primera será principal
        //        };

        //        product.Images.Add(image);
        //        urls.Add(result.Url);
        //    }

        //    product.UpdatedAt = DateTime.UtcNow;

        //    await _productAccessBd.UpdateAsync(product);

        //    return urls;
        //}

        public async Task<bool> DeleteProductImage(int productId, string publicId)
        {
            var product = await _productAccessBd.GetByIdAsync(productId);

            if (product == null)
                return false;

            var image = product.Images.FirstOrDefault(i => i.PublicId == publicId);

            if (image == null)
                return false;

            // eliminar en cloudinary
            await _cloudinaryService.DeleteImageAsync(publicId);

            // eliminar de la BD
            product.Images.Remove(image);

            await _productAccessBd.UpdateAsync(product);

            return true;
        }

        Task<List<string>> IProductService.ReplaceProductImages(int productId, IFormFile[] files)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductImageDTO?> ReplaceImage(int productId, string oldPublicId, IFormFile newFile)
        {
            var product = await _productAccessBd.GetByIdAsync(productId);

            if (product == null)
                return null;

            var oldImage = product.Images.FirstOrDefault(i => i.PublicId == oldPublicId);

            if (oldImage == null)
                return null;

            // borrar imagen vieja
            await _cloudinaryService.DeleteImageAsync(oldPublicId);

            string datePart = DateTime.UtcNow.ToString("yyMMdds");
            string publicId = $"{product.Code}_{datePart}";
            // subir nueva
            var uploadResult = await _cloudinaryService.UploadImage(newFile, publicId+"r");

            oldImage.Url = uploadResult.Url;
            oldImage.PublicId = uploadResult.PublicId;

            await _productAccessBd.UpdateAsync(product);

            return new ProductImageDTO
            {
                Url = oldImage.Url,
                PublicId = oldImage.PublicId,
                IsMain = oldImage.IsMain
            };
        }

        public async Task<ImportResult> ImportProductsFullExcel(IFormFile file)
        {
            // Leer Excel
            var importResult = await _productExcelService.ImportProductsFullExcel(file);

            // Guardar productos en BD
            var saveResult = await SaveImportedProductsAsync(importResult.Products);

            return saveResult;
        }
        public async Task<ImportResult> ImportStockProductExcell(IFormFile file)
        {
            // Leer Excel
            var importResult = await _productExcelService.ImportStockProductsExcel(file);

            // Update productos en BD
            //var saveResult = await SaveImportedProductsAsync(importResult.Products);

            return importResult;
        }

        public async Task<ImportResult> SaveImportedProductsAsync(List<ProductDTO> products)
        {
            var result = new ImportResult();
            result.Products = new List<ProductDTO>();

            foreach (var productDto in products)
            {
                // Normalizar código
                productDto.Code = productDto.Code.Trim().ToUpper();

                // Verificar si ya existe
                var exists = await _context.Products.AnyAsync(p => p.Code == productDto.Code);
                if (exists)
                {
                    result.Duplicates++;
                    continue;
                }

                // Buscar o crear categoría
                CategoryEntity category = null;
                if (!string.IsNullOrEmpty(productDto.CategoryName))
                {
                    category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == productDto.CategoryName);
                    if (category == null)
                    {
                        category = new CategoryEntity { Name = productDto.CategoryName };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }
                }

                // Mapear DTO a Entity
                var entity = new ProductEntity
                {
                    Code = productDto.Code,
                    Barcode = productDto.Barcode,
                    Name = productDto.Name,
                    Brand = productDto.Brand,
                    Model = productDto.Model,
                    Price = productDto.Price,
                    Stock = productDto.Stock,
                    MinStock = productDto.MinStock,
                    Description = productDto.Description,
                    IsActive = productDto.IsActive,
                    CategoryId = category?.Id ?? 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Images = new List<ProductImageEntity>() // se llenarán luego
                };

                // Mapear LedDetails si existen
                if (productDto.LedDetails != null)
                {
                    entity.LedDetails = new LedStripDetailsEntity
                    {
                        Inch = productDto.LedDetails.Inch,
                        StripCount = productDto.LedDetails.StripCount,
                        LengthMm = productDto.LedDetails.LengthMm,
                        LedCount = productDto.LedDetails.LedCount,
                        LedVolts = productDto.LedDetails.LedVolts,
                        BoardCode = productDto.LedDetails.BoardCode,
                        Distribution = productDto.LedDetails.Distribution,
                        LedType = productDto.LedDetails.LedType,
                        Notes = productDto.LedDetails.Notes,
                        CompatibleTVs = productDto.LedDetails.CompatibleTVModels?
                            .Select(m => new TVModelEntity { ModelCode = m })
                            .ToList() ?? new List<TVModelEntity>()
                    };
                }

                // Guardar producto
                _context.Products.Add(entity);
                await _context.SaveChangesAsync();

                // Mapear ID generado al DTO
                productDto.Id = entity.Id;
                productDto.CategoryId = entity.CategoryId;
                result.Products.Add(productDto);
                result.Created++;
            }

            return result;
        }

        public async Task<ProductDTO?> UpdateInfo(int id, UpdateProductInfoDTO dto)
        {
            var product = await _productAccessBd.GetByIdAsync(id);

            if (product == null)
                return null;

            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Model = dto.Model;
            product.Barcode = dto.Barcode;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.MinStock = dto.MinStock;


            product.Description = dto.Description;
            product.IsActive = dto.IsActive;

            if (product.LedDetails != null && dto.LedDetails != null)
            {
                product.LedDetails.Inch = dto.LedDetails.Inch;
                product.LedDetails.StripCount = dto.LedDetails.StripCount;
                product.LedDetails.LengthMm = dto.LedDetails.LengthMm;
                product.LedDetails.LedCount = dto.LedDetails.LedCount;
                product.LedDetails.LedVolts = dto.LedDetails.LedVolts;
                product.LedDetails.LedType = (LedType) dto.LedDetails.LedType;
                product.LedDetails.BoardCode = dto.LedDetails.BoardCode;
                product.LedDetails.Distribution = dto.LedDetails.Distribution;
                product.LedDetails.Notes = dto.LedDetails.Notes;

                product.LedDetails.CompatibleTVs ??= new List<TVModelEntity>();

                var led = product.LedDetails.CompatibleTVs; //led ya es una referencia a la lista 
                led.Clear();

                if (dto.LedDetails.CompatibleTVModels != null)
                {
                    foreach (var model in dto.LedDetails.CompatibleTVModels)
                    {
                        led.Add(new TVModelEntity
                        {
                            ModelCode = model
                        });
                    }
                }
            }

            product.UpdatedAt = DateTime.UtcNow;
            await _productAccessBd.UpdateAsync(product);
            return _mapper.Map<ProductDTO>(product);
        }
    }


 }