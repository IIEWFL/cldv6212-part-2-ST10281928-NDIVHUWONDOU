using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services
{
    public class FunctionService
    {
        
        private readonly HttpClient _httpClient;
        private readonly string _functionBaseUrl;
        public FunctionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _functionBaseUrl = configuration["AzureFunctionsBaseUrlProd"] ?? throw new InvalidOperationException("Azure function base URL is missing");
        }

        //Get All Customers
        public async Task<List<Customer>>GetCustomersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Customer>>($"{_functionBaseUrl}/api/customers" );
            return response ?? new List<Customer>();
        }

        //Create Student
        public async Task<bool> AddProductAsync(Product product, IFormFile photo)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(product.ProductName ?? string.Empty), "Name");
            content.Add(new StringContent(product.ProductDescription ?? string.Empty), "Description");
            content.Add(new StringContent(product.Price.ToString()), "Price");
            content.Add(new StringContent(product.Stock.ToString()), "Stock");

            if(photo != null)
            {
                var streamContent = new StreamContent(photo.OpenReadStream());
                content.Add(streamContent, "photo", photo.FileName);
            }
            var response = await _httpClient.PostAsync($"{_functionBaseUrl}/api/products", content);
            return response.IsSuccessStatusCode;

        }

        //Get queue log
        public async Task<List<QueueViewLog>> GetMessagesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<QueueViewLog>>($"{_functionBaseUrl}/api/systemlog");
            return response ?? new List<QueueViewLog>();
        }

        //Upload to Fileshare
        public async Task<string> ExportLog(string name)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_functionBaseUrl}/api/system-log", new { name } );
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
