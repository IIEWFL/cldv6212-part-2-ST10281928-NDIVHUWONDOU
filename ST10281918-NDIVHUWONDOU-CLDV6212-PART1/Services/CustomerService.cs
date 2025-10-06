using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services
{
    public class CustomerService
    {
        private readonly TableStorageService<Customer> _tableService;

        // Inject both TableStorageService and BlobStorageService
        public CustomerService(string tableConnectionString, string tableName)
        {
            _tableService = new TableStorageService<Customer>(tableConnectionString, tableName);  
        }

        public Task<List<Customer>> GetAllCustomersAsync()
            => _tableService.GetAllAsync();

        public Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
            => _tableService.GetAsync(partitionKey, rowKey);

        // Add product with optional image
        public async Task AddCustomerAsync(Customer customer, Stream? imageStream = null, string? fileName = null)
        {

            await _tableService.AddAsync(customer);
        }

        // Update product and optionally replace image
        public async Task UpdateCustomerAsync(Customer customer, Stream? newImageStream = null, string? newFileName = null)
        {


            await _tableService.UpdateAsync(customer);
        }

        public Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            // Optionally: delete image as well
            return _tableService.DeleteAsync(partitionKey, rowKey);
        }
    }
}
