using Azure;
using Azure.Data.Tables;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models
{
    public class Customer :ITableEntity
    {
        //Code Attribution
        //This method was taken from the Microsoft website
        //https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-model?view=aspnetcore-9.0&tabs=visual-studio
        //Microsoft

        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string? CustomerID { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
    }
}
