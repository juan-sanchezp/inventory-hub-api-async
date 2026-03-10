using AutoMapper;
using CloudinaryDotNet;
using ExcelDataReader;
using InventoryHub.Data;
using InventoryHub.DTOs;
using InventoryHub.Enums;
using InventoryHub.Models;
using InventoryHub.Repositories;
using InventoryHub.Services.CloudinaryS;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _productAccessBd;
        private readonly ICategoryRepository _categoryAccessBd;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public ProductServiceImpl(IProductRepository productRepository, ICategoryRepository categoryAccessBd, IMapper mapper, AppDbContext context, CloudinaryService cloudinaryService)
        {
            _productAccessBd = productRepository;
            _categoryAccessBd = categoryAccessBd;
            _mapper = mapper;
            _context = context;
            _cloudinaryService = cloudinaryService;
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
                CompatibleTVModel = dto.CompatibleTVModel,
                MinLedCount = dto.MinLedCount,
                MaxLedCount = dto.MaxLedCount,
                MinLengthMm = dto.MinLengthMm,
                MaxLengthMm = dto.MaxLengthMm,
                LedVolts = dto.LedVolts
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

            foreach (var file in files)
            {
                // Subir a Cloudinary
                var result = await _cloudinaryService.UploadImage(file);

                // Crear entidad imagen
                var image = new ProductImageEntity
                {
                    Url = result.Url,
                    PublicId = result.PublicId,
                    ProductId = productId,
                    IsMain = product.Images.Count == 0 // la primera será principal
                };

                product.Images.Add(image);
                urls.Add(result.Url);
            }

            product.UpdatedAt = DateTime.UtcNow;

            await _productAccessBd.UpdateAsync(product);

            return urls;
        }

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

            // subir nueva
            var uploadResult = await _cloudinaryService.UploadImage(newFile);

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
            var result = new ImportResult
            {
                Products = new List<ProductDTO>()
            };

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var conf = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            };

            var dataSet = reader.AsDataSet(conf);
            var table = dataSet.Tables[0];

            // Mantener códigos para detectar duplicados dentro del mismo archivo
            var existingCodes = new HashSet<string>();

            foreach (System.Data.DataRow row in table.Rows)
            {
                string code = row["code"]?.ToString().Trim() ?? "";
                if (string.IsNullOrEmpty(code) || existingCodes.Contains(code))
                {
                    result.Duplicates++;
                    continue;
                }
                existingCodes.Add(code);

                string categoryName = row["categoryName"]?.ToString().Trim() ?? "Sin categoría";

                // Crear DTO
                var productDto = new ProductDTO
                {
                    Id = 0, // se asignará al guardar en BD
                    Code = code,
                    Barcode = row["barcode"]?.ToString() ?? "",
                    Name = row["name"]?.ToString() ?? "Sin nombre",
                    Brand = row["brand"]?.ToString() ?? "Sin marca",
                    Model = row["model"]?.ToString(),
                    Price = decimal.TryParse(row["price"]?.ToString(), out var price) ? price : 0,
                    Stock = int.TryParse(row["stock"]?.ToString(), out var stock) ? stock : 0,
                    MinStock = int.TryParse(row["minStock"]?.ToString(), out var minStock) ? minStock : 0,
                    Description = row["description"]?.ToString(),
                    CategoryId = 0, // se asigna al guardar en BD
                    CategoryName = categoryName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Images = new List<ProductImageDTO>(), // se llenarán después
                    LedDetails = null
                };

                // Crear LedDetails solo si hay datos
                var inchStr = row["inch"]?.ToString();
                if (!string.IsNullOrEmpty(inchStr))
                {
                    // Leer LedType desde Excel
                    LedType ledTypeValue = LedType.Normal;
                    var ledTypeStr = row["ledType"]?.ToString();
                    if (!string.IsNullOrEmpty(ledTypeStr) && int.TryParse(ledTypeStr, out int ledTypeInt))
                    {
                        if (Enum.IsDefined(typeof(LedType), ledTypeInt))
                            ledTypeValue = (LedType)ledTypeInt;
                    }

                    productDto.LedDetails = new LedStripDetailsDTO
                    {
                        Inch = int.TryParse(row["inch"]?.ToString(), out var inch) ? inch : 0,
                        StripCount = int.TryParse(row["stripCount"]?.ToString(), out var stripCount) ? stripCount : 0,
                        LengthMm = int.TryParse(row["lengthMm"]?.ToString(), out var lengthMm) ? lengthMm : null,
                        LedCount = int.TryParse(row["ledCount"]?.ToString(), out var ledCount) ? ledCount : null,
                        LedVolts = int.TryParse(row["ledVolts"]?.ToString(), out var ledVolts) ? ledVolts : null,
                        BoardCode = row["boardCode"]?.ToString(),
                        Distribution = row["distribution"]?.ToString(),
                        LedType = ledTypeValue, // ✅ enum seguro
                        Notes = row["notes"]?.ToString(),
                        CompatibleTVModels = string.IsNullOrEmpty(row["compatibleTVModels"]?.ToString())
                            ? new List<string>()
                            : row["compatibleTVModels"]?.ToString()
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .ToList()
                    };
                }

                result.Products.Add(productDto);
                result.Created++;
            }

            return result;
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



    }


 }