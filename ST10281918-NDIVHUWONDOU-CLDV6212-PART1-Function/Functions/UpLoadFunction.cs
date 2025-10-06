
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Functions
{
    public class UpLoadFunction
    {
        private readonly QueueStorageService _queueStorageService;
        private readonly FileShareStorageService _fileShareStorageService;

        public UpLoadFunction(QueueStorageService queueStorageService, FileShareStorageService fileShareStorageService)
        {
            _queueStorageService = queueStorageService;
            _fileShareStorageService = fileShareStorageService;
        }
        [FunctionName("UpLoad")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "system-log")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to upload a log file.");

            //retrieve file name
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string name = data?.name;

            try
            {
                //get all messages
                List<QueueViewLog> logMessages = await _queueStorageService.GetLogEntriesAsync();
                //create CSV content
                var content = new StringBuilder();
                content.AppendLine("MessageId, InsertionTime, MessageText");
                foreach (var logMessage in logMessages)
                {
                    var msgText = logMessage.RawMessage.Replace("\"", "\"\"");
                    content.AppendLine($"{logMessage.MessageId},{logMessage.InsertionTime},\"{logMessage.RawMessage?.Replace("\"", "\"\"")}\"");

                }
                //upload 
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())))
                {
                    await _fileShareStorageService.UpLoadFile(name, stream);
                }
                //clear queue

                await _queueStorageService.ClearLogsAsync();
                return new OkObjectResult($"Log file '{name}' uploaded successfully");
            }
            catch (Exception ex)
            {
                log.LogError($"Error uploading log file: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        
    }
    }
}
