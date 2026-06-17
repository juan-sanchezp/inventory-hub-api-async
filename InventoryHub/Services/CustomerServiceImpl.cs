using AutoMapper;
using InventoryHub.DTOs.Customer;
using InventoryHub.Models;
using InventoryHub.Repositories;


namespace InventoryHub.Services
{
    public class CustomerServiceImpl : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerServiceImpl(
            ICustomerRepository customerRepository,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        // Obtener todos los clientes
        public async Task<List<CustomerDTO>> GetAll()
        {
            var customersEntity = await _customerRepository.GetAllAsync();

            return _mapper.Map<List<CustomerDTO>>(customersEntity);
        }

        // Obtener cliente por Id
        public async Task<CustomerDTO?> GetById(int id)
        {
            var customerEntity = await _customerRepository.GetByIdAsync(id);

            if (customerEntity == null)
                return null;

            return _mapper.Map<CustomerDTO>(customerEntity);
        }

        // Guardar cliente
        public async Task<CustomerDTO?> Save(CustomerDTO customerDTO)
        {
            var customerEntity = _mapper.Map<CustomerEntity>(customerDTO);

            var savedEntity = await _customerRepository.AddAsync(customerEntity);

            if (savedEntity == null)
                return null;

            return _mapper.Map<CustomerDTO>(savedEntity);
        }

        // Actualizar cliente
        public async Task<CustomerDTO?> Update(int id, CustomerDTO customerDTO)
        {
            var customerEntity = await _customerRepository.GetByIdAsync(id);

            if (customerEntity == null)
                return null;

            _mapper.Map(customerDTO, customerEntity);

            customerEntity.CreatedAt = DateTime.UtcNow;

            var updatedEntity = await _customerRepository.UpdateAsync(customerEntity);

            return _mapper.Map<CustomerDTO>(updatedEntity);
        }

        // Eliminar cliente
        public async Task<CustomerDTO?> DeleteById(int id)
        {
            var customerEntity = await _customerRepository.GetByIdAsync(id);

            if (customerEntity == null)
                return null;

            bool deleted = await _customerRepository.DeleteAsync(customerEntity);

            if (!deleted)
                return null;

            return _mapper.Map<CustomerDTO>(customerEntity);
        }
    }
}