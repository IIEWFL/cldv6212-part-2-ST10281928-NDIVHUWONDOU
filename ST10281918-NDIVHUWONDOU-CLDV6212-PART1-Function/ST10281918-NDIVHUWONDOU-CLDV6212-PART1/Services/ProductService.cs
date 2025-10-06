using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services
{
    public class ProductService
    {
        private readonly TableStorageService<Product> _tableService;
        private readonly BlobStorageService _blobService;

        // Inject both TableStorageService and BlobStorageService
        public ProductService(string tableConnectionString, string tableName, BlobStorageService blobService)
        {
            _tableService = new TableStorageService<Product>(tableConnectionString, tableName);
            _blobService = blobService;
        }

        public Task<List<Product>> GetAllProductAsync()
            => _tableService.GetAllAsync();

        public Task<Product?> GetProductAsync(string partitionKey, string rowKey)
            => _tableService.GetAsync(partitionKey, rowKey);

        // Add product with optional image
        public async Task AddProductAsync(Product product, Stream? imageStream = null, string? fileName = null)
        {
            if (imageStream != null && fileName != null)
            {
                product.Photo = await _blobService.UploadImageAsync(imageStream, fileName);
            }

            await _tableService.AddAsync(product);
        }

        // Update product and optionally replace image
        public async Task UpdateProductAsync(Product product, Stream? newImageStream = null, string? newFileName = null)
        {
            if (newImageStream != null && !string.IsNullOrEmpty(newFileName))
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.Photo))
                {
                    await _blobService.DeleteImageAsync(product.Photo);
                }

                // Upload new image
                product.Photo = await _blobService.UploadImageAsync(newImageStream, newFileName);
            }

            await _tableService.UpdateAsync(product);
        }

        public Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            // Optionally: delete image as well
            return _tableService.DeleteAsync(partitionKey, rowKey);
        }
    }
}
