using Microsoft.AspNetCore.Mvc;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;
using System.Text;
using System.Threading.Tasks;
namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Controllers
{
    public class LogController : Controller
    {
        private readonly FunctionService _functionService;
        private readonly QueueStorageService _queueStorageService;
        private readonly FileShareStorageService _fileShareService;
        public LogController(QueueStorageService queueStorageService, FileShareStorageService fileShareService, FunctionService functionService)
        {
            _queueStorageService = queueStorageService;
            _fileShareService = fileShareService;
            _functionService = functionService;
        }
        public async Task<IActionResult> Index()
        {
            var log = await _functionService.GetMessagesAsync();
            return View(log);
        }

        //Code Attribution
        //This method was taken form stackoverflow
        //https://stackoverflow.com/questions/18757097/writing-data-into-csv-file-in-c-sharp
        //TylerH and Pavel Murygin
        //https://stackoverflow.com/users/2756409/tylerh
        //https://stackoverflow.com/users/731793/pavel-murygin


        // POST: /AuditLogs/Export
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export()
        {
            //var logs = await _queueStorageService.GetLogEntriesAsync();
            var fileName = $"QueueLog_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            
            var responseMessages = await _functionService.ExportLog(fileName);
            // Clear logs
            //await _queueStorageService.ClearLogsAsync();
            
            // Redirect to download route
            return RedirectToAction("Download");
            
        }

        public IActionResult Download()
        {
            if (TempData["FileName"] != null && TempData["FileBytes"] != null)
            {
                var fileName = TempData["FileName"].ToString();
                var fileBytes = Convert.FromBase64String(TempData["FileBytes"].ToString());

                // Redirects after file download
                Response.Headers.Add("Refresh", "1;url=/Log/Index");
                return File(fileBytes, "text/csv", fileName);
            }

            // fallback
            return RedirectToAction("Index");
        }






    }
}
