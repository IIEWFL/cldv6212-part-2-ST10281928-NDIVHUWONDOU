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

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Functions.CustomerFunc
{
    public class CreateCustomerFunction
    {
        private readonly CustomerService _customerService;
        private readonly QueueStorageService _queueStorageService;
        public CreateCustomerFunction(CustomerService customerService, QueueStorageService queueStorageService)
        {
            _customerService = customerService;
            _queueStorageService = queueStorageService;
        }
        [FunctionName("CreateCustomer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to create a new customer.");

            //set partitionkey to customer
            var form = await req.ReadFormAsync();
            var partitionkey = "Customer";
            var rowkey = Guid.NewGuid().ToString();
            var customer = new Models.Customer
            {
                PartitionKey = partitionkey,
                RowKey = rowkey,
                CustomerFirstName = form["name"],
                CustomerLastName = form["surname"],
                CustomerEmail = form["email"],
                CustomerPhone = form["phone"]
            };
            log.LogInformation($"Creating student with Partitionkey: {partitionkey}, Rowkey: {rowkey}");

            //validate
            if (string.IsNullOrEmpty(customer.PartitionKey) || string.IsNullOrEmpty(customer.RowKey))
            {
                return new BadRequestObjectResult("Partitionkey and rowkey cannot be empty or null");
            }
            await _customerService.AddCustomerAsync(customer);
            //SendFileResponseExtensions message to your queue
            await _queueStorageService.SendMessagesAsync(new { Action = "Create", Customer = customer });

            return new OkObjectResult(customer);

        }
    }
}
