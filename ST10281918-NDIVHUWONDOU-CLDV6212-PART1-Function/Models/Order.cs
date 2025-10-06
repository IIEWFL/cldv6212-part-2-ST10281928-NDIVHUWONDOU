﻿#nullable enable
using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Models
{
    public class Order : ITableEntity
    {
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? ProductId { get; set; }
        public int? Quantity { get; set; }
        public string? OrderStatus { get; set; }

    }
}
