using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Functions.CustomerFunc
{
    public class GetCustomersFunction
    {
        private readonly CustomerService _tableService;

        public GetCustomersFunction(CustomerService tableService)
        {
            _tableService = tableService;
        }
        [FunctionName("GetCustomers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to get all the customers.");

            //retrieve all students
            var customers = await _tableService.GetAllCustomersAsync();

            //convert students to a studentdto
            var customerdto = customers.Select(c => new CustomerDTO
            {
                PartitionKey = c.PartitionKey,
                RowKey = c.RowKey,
                Timestamp = c.Timestamp,
                ETag = c.ETag.ToString(),
                CustomerID = c.CustomerID,
                CustomerFirstName = c.CustomerFirstName,
                CustomerEmail = c.CustomerEmail,
                CustomerLastName = c.CustomerLastName,
                CustomerPhone = c.CustomerPhone,

            }).ToList();
                //PartitionKey = s.PartitionKey,
                //RowKey = s.RowKey,
                //Timestamp = s.Timestamp,
                //ETag = s.ETag.ToString(),
                //PhotoUrl = s.PhotoUrl,
                //StudentNumber = s.StudentNumber,
                //Name = s.Name,
                //Email = s.Email,
                //Faculty = s.Faculty,

            //return list as a API response
            return new ObjectResult(customerdto);
        }
    }
}
