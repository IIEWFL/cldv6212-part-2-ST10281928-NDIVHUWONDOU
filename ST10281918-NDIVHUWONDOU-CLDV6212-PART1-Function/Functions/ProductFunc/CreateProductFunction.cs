using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Functions.ProductFunc
{
    public class CreateProductFunction
    {
        private readonly ProductService _tableStorageService;
        private readonly BlobStorageService _blobStorageService;
        private readonly QueueStorageService _queueStorageService;
        public CreateProductFunction(ProductService tableStorageService, BlobStorageService blobStorageService, QueueStorageService queueStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _queueStorageService = queueStorageService;
        }
        [FunctionName("CreateProduct")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "products")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to create a product.");

            //set partitionhey to product
            var form = await req.ReadFormAsync();
            var partitionkey = "Product";
            var rowkey = Guid.NewGuid().ToString();
            var product = new Product
            {
                PartitionKey = partitionkey,
                RowKey = rowkey,
                ProductId = Guid.NewGuid().ToString(),
                ProductName = form["name"],
                ProductDescription = form["description"],
                Price = double.Parse(form["price"]),
                Stock = int.Parse(form["stock"])
            };
            log.LogInformation($"Creating student with Partitionkey: {partitionkey}, Rowkey: {rowkey}");

            //handle your photos
            if (req.Form.Files.Count > 0)
            {
                var photo = req.Form.Files[0];
                using var stream = photo.OpenReadStream();
                product.Photo = await _blobStorageService.UploadImageAsync(stream, Guid.NewGuid().ToString());
            }
            //validate
            if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
            {
                return new BadRequestObjectResult("Partitionkey and rowkey cannot be empty or null");
            }
            await _tableStorageService.AddProductAsync(product);
            //SendFileResponseExtensions message to your queue
            await _queueStorageService.SendMessagesAsync(new { Action = "Create", Product = product });

            return new OkObjectResult(product);
        }
    }
}
