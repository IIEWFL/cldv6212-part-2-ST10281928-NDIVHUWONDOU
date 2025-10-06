namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models
{
    public class QueueViewLog
    {
            public string? MessageId { get; set; }
            public DateTimeOffset? InsertionTime { get; set; }

            // From the JSON payload
            public string? Action { get; set; }
            public string? Entity { get; set; }
            public string? CustomerId { get; set; }
            public string? CustomerName { get; set; }
            public DateTime? Timestamp { get; set; }

            // Fallback
            public string? RawMessage { get; set; }
        }
    }
