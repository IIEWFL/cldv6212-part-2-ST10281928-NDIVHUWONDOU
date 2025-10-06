//using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
//using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;

//namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services
//{
//    public class OrderService
//    {
//        private readonly TableStorageService<Order> _tableService;
//        //private readonly BlobStorageService _blobService;

//        // Inject both TableStorageService and BlobStorageService
//        public OrderService(string tableConnectionString, string tableName)
//        {
//            _tableService = new TableStorageService<Order>(tableConnectionString, tableName);

//        }

//        public Task<List<Order>> GetAllOrdersAsync()
//            => _tableService.GetAllAsync();

//        public Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
//            => _tableService.GetAsync(partitionKey, rowKey);

//        // Add product with optional image
//        public async Task AddOrderAsync(Order order, Stream? imageStream = null, string? fileName = null)
//        {

//            await _tableService.AddAsync(order);
//        }

//        // Update product and optionally replace image
//        public async Task UpdateOrderAsync(Order order, Stream? newImageStream = null, string? newFileName = null)
//        {


//            await _tableService.UpdateAsync(order);
//        }

//        public Task DeleteOrderAsync(string partitionKey, string rowKey)
//        {
//            // Optionally: delete image as well
//            return _tableService.DeleteAsync(partitionKey, rowKey);
//        }
//    }
//}
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services
{
    public class OrderService
    {
        private readonly TableStorageService<Order> _tableService;

        public OrderService(string connectionString, string tableName)
        {
            _tableService = new TableStorageService<Order>(connectionString, tableName);
        }

        public Task<List<Order>> GetAllOrdersAsync()
            => _tableService.GetAllAsync();

        public Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
            => _tableService.GetAsync(partitionKey, rowKey);

        public Task AddOrderAsync(Order order)
            => _tableService.AddAsync(order);


        public Task UpdateOrderAsync(Order order)

            => _tableService.UpdateAsync(order);

        public Task DeleteOrderAsync(string partitionKey, string rowKey)
            => _tableService.DeleteAsync(partitionKey, rowKey);
    }
}

