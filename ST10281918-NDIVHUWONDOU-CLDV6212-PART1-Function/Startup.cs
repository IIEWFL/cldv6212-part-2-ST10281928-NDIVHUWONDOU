using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services;
using System;

[assembly: FunctionsStartup(typeof(ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Startup))]

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function
{
    public class Startup : FunctionsStartup
    {
        //reg all services
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(sp => CreateStorageServices<CustomerService>(sp, "Customer", "tableCustomer"));
            builder.Services.AddSingleton(sp => CreateStorageServices<OrderService>(sp, "Order", "tableOrder"));
            builder.Services.AddSingleton(sp => CreateStorageServices<ProductService>(sp, "Product", "tableProduct"));
            
            builder.Services.AddSingleton(sp =>
            CreateStorageServices<BlobStorageService>(sp, "productphotos", "blob"));

            builder.Services.AddSingleton(sp =>
            CreateStorageServices<QueueStorageService>(sp, "orderqueue", "queue"));

            builder.Services.AddSingleton(sp =>
            CreateStorageServices<FileShareStorageService>(sp, "retail-log-file", "fileshare"));

            

        }

        //helper method
        private T CreateStorageServices<T>(IServiceProvider sp, string serviceidentifier, string servicetype) where T : class
        {
            var logger = sp.GetRequiredService<ILogger<T>>();
            var configuration = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var connectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            if (string.IsNullOrEmpty(connectionstring) || string.IsNullOrWhiteSpace(serviceidentifier))
            {
                logger.LogError("Storage connection string or service identifier is not found");
                throw new InvalidOperationException("Configuration is invalid");

            }
            return servicetype switch
            {
                "tableCustomer" => new CustomerService(connectionstring, serviceidentifier) as T,
                "tableOrder" => new OrderService(connectionstring, serviceidentifier) as T,
                "blob" => new BlobStorageService(connectionstring, serviceidentifier) as T,
                "tableProduct" => new ProductService(connectionstring, serviceidentifier, new BlobStorageService(connectionstring, "productphotos")) as T,
                "queue" => new QueueStorageService(connectionstring, serviceidentifier) as T,
                "fileshare" => new FileShareStorageService(connectionstring, serviceidentifier) as T
            };
        }
    }
}
