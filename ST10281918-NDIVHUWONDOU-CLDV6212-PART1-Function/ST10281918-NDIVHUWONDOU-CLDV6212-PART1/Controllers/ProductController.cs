using Microsoft.AspNetCore.Mvc;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Controllers
{
    public class ProductController : Controller
    {
        private readonly FunctionService _functionService;
        private readonly ProductService _productService;
        private readonly BlobStorageService _blobStorageService;
        private readonly QueueStorageService _queueStorageService;
        private readonly FileShareStorageService _fileShareService;

        public ProductController(ProductService productService, BlobStorageService blobStorageService, QueueStorageService queueStorageService, FileShareStorageService fileShareStorageService, FunctionService functionService)
        {
            _productService = productService;
            _blobStorageService = blobStorageService;
            _queueStorageService = queueStorageService;
            _fileShareService = fileShareStorageService;
            _functionService = functionService;
        }
        public async Task<IActionResult> Index()
        {
            var product = await _productService.GetAllProductAsync();
            foreach (var a in product)
            {
                if (!string.IsNullOrEmpty(a.Photo))
                    a.ImageSasUrl = _blobStorageService.GetImageSasUri(a.Photo);
            }
            return View(product);
        }

        //GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }
        //Code Attribution
        //This method was taken from the Miscrosoft website
        //https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload
        //Microsoft

        //POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? photo)
        {
            if (ModelState.IsValid)
            {
                await _functionService.AddProductAsync(product, photo);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Details
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var product = await _productService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.Photo))
            {
                product.ImageSasUrl = _blobStorageService.GetImageSasUri(product.Photo);
            }

            return View(product);
        }

        // GET: /Artists/Edit/{partitionKey}/{rowKey}
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var product = await _productService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
                return NotFound();

            // Generate SAS URL for current image if exists
            if (!string.IsNullOrEmpty(product.Photo))
            {
                product.ImageSasUrl = _blobStorageService.GetImageSasUri(product.Photo);
            }

            return View(product);
        }

        // POST: /Artists/Edit/{partitionKey}/{rowKey}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? newImageFile)
        {
            if (!ModelState.IsValid)
                return View(product);

            // Retrieve the existing artist to preserve old image if needed
            var existingArtist = await _productService.GetProductAsync(product.PartitionKey!, product.RowKey!);
            if (existingArtist == null)
                return NotFound();

            if (newImageFile != null && newImageFile.Length > 0)
            {
                // Upload new image and replace old image
                using var stream = newImageFile.OpenReadStream();
                await _productService.UpdateProductAsync(product, stream, newImageFile.FileName);
            }
            else
            {
                // Preserve old image if no new image uploaded
                product.Photo = existingArtist.Photo;
                await _productService.UpdateProductAsync(product);
            }

            // Generate SAS URL for display after edit (optional)
            if (!string.IsNullOrEmpty(product.Photo))
            {
                product.ImageSasUrl = _blobStorageService.GetImageSasUri(product.Photo);
            }
            // Audit log
            await _queueStorageService.SendMessagesAsync(new
            {
                Action = "Edit Product",
                Entity = "Product",
                ProductName = product.ProductName,
                ProductRowKey = product.RowKey,
                Timestamp = DateTime.UtcNow
            });

            return RedirectToAction(nameof(Index));
        }

        // GET: /Artists/Delete/{partitionKey}/{rowKey}
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var product = await _productService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
                return NotFound();

            if (!string.IsNullOrEmpty(product.Photo))
            {
                product.ImageSasUrl = _blobStorageService.GetImageSasUri(product.Photo);
            }

            return View(product);
        }

        // POST: /Artists/DeleteConfirmed/{partitionKey}/{rowKey}
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            var product = await _productService.GetProductAsync(partitionKey, rowKey);

            if (product != null)
            {
                // Delete image first
                if (!string.IsNullOrEmpty(product.Photo))
                    await _blobStorageService.DeleteImageAsync(product.Photo);

                await _productService.DeleteProductAsync(partitionKey, rowKey);

            }

            // Audit log
            await _queueStorageService.SendMessagesAsync(new
            {
                Action = "Delete Product",
                Entity = "Product",
                ProductName = product.ProductName,
                ProductRowKey = product.RowKey,
                Timestamp = DateTime.UtcNow
            });

            return RedirectToAction(nameof(Index));
        }
    }
}
//var productValue = new Product
//{
//    PartitionKey = "product",
//    RowKey = Guid.NewGuid().ToString(),
//    ProductId = Guid.NewGuid().ToString(),
//    ProductName = product.ProductName,
//    ProductDescription = product.ProductDescription,
//    Price = product.Price,
//    Stock = product.Stock
//};