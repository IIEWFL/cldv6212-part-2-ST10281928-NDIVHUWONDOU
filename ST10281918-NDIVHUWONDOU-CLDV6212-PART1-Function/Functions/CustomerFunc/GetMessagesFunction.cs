using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Functions.CustomerFunc
{
    public class GetMessagesFunction
    {
        private readonly QueueStorageService _queueStorageService;

        public GetMessagesFunction(QueueStorageService queueStorageService)
        {
            _queueStorageService = queueStorageService;
        }
        [FunctionName("GetMessages")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "systemlog")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var messages = await _queueStorageService.GetLogEntriesAsync();
            return new OkObjectResult(messages);
        }
    }
}
