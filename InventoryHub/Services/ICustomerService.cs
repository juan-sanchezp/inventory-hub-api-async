using InventoryHub.DTOs.Customer;
namespace InventoryHub.Services

{
    public interface ICustomerService
    {

        // basic Crud operations
        Task<List<CustomerDTO>> GetAll();
        Task<CustomerDTO?> GetById(int id);
        Task<CustomerDTO?> Save(CustomerDTO customerDTO);
        Task<CustomerDTO?> Update(int id, CustomerDTO customerDTO);
        Task<CustomerDTO?> DeleteById(int id);

        // Search methods

        ////Task<List<CustomerDTO>> SearchByName(string searchTerm);

        //// Estadísticas
        //Task<int> GetTotalCustomersCount();
        //Task<List<CustomerDTO>> GetRecentCustomers(int days);
        //// Importación/Exportación (si aplica para clientes)
        //Task<ImportResult> ImportCustomersFromExcel(IFormFile file);
        //Task<byte[]> ExportCustomersToExcel();
    }
}
