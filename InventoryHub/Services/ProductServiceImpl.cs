using AutoMapper;
using InventoryHub.DTOs;
using InventoryHub.Models;
using InventoryHub.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryHub.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductServiceImpl(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetAll()
        {
            var productsEntity = await _productRepository.GetAllAsync();
            return _mapper.Map<List<ProductDTO>>(productsEntity);
        }

        public async Task<ProductDTO?> GetById(int id)
        {
            var productEntity = await _productRepository.GetByIdAsync(id);
            if (productEntity == null) return null;

            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task<ProductDTO?> Save(ProductDTO productDTO)
        {
            var entity = _mapper.Map<ProductEntity>(productDTO);

            // Llama al repo async con validación de duplicados incluida
            var saved = await _productRepository.AddAsync(entity);
            if (saved == null) return null;

            return _mapper.Map<ProductDTO>(saved);
        }

        public async Task<ProductDTO?> Update(int id, ProductDTO productDTO)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return null;

            // Actualiza propiedades del entity con AutoMapper
            _mapper.Map(productDTO, existingProduct);

            var updated = await _productRepository.UpdateAsync(existingProduct);
            return _mapper.Map<ProductDTO>(updated);
        }

        public async Task<ProductDTO?> DeleteById(int id)
        {
            var productEntity = await _productRepository.GetByIdAsync(id);
            if (productEntity == null) return null;

            await _productRepository.DeleteAsync(productEntity);
            return _mapper.Map<ProductDTO>(productEntity);
        }
    }
}