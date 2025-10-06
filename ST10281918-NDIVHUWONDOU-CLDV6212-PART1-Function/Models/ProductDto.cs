#nullable enable
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Models
{
    public class ProductDto
    {
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string? ETag { get; set; }
        public string? ProductId { get; set; }
        public string? ImageSasUrl { get; set; }
        public string? Photo { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public double? Price { get; set; }
        public int? Stock { get; set; }
    }
}
