using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queueClient;

        public QueueStorageService(string storageAccount, string queueName)
        {
            var serviceClient = new QueueServiceClient(storageAccount);
            _queueClient = serviceClient.GetQueueClient(queueName);
            _queueClient.CreateIfNotExists();
        }
        public async Task SendMessagesAsync(object message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            await _queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson)));
        }

        //Get log entries from queue
        public async Task<List<QueueViewLog>> GetLogEntriesAsync()
        {
            var entryList = new List<QueueViewLog>();
            var entries = await _queueClient.PeekMessagesAsync(maxMessages: 32);

            foreach (PeekedMessage entry in entries.Value)
            {
                string rawMessage = entry.Body.ToString();
                string decodedJson = string.Empty;

                try
                {
                    // Decode Base64 -> JSON
                    decodedJson = Encoding.UTF8.GetString(Convert.FromBase64String(rawMessage));

                    var deserialized = JsonSerializer.Deserialize<QueueViewLog>(decodedJson);

                    if (deserialized != null)
                    {
                        deserialized.MessageId = entry.MessageId;
                        deserialized.InsertionTime = entry.InsertedOn;
                        deserialized.RawMessage = decodedJson;
                        entryList.Add(deserialized);
                        continue;
                    }
                }
                catch
                {
                    // ignore exception and fallback
                }

                // fallback if deserialization failed
                entryList.Add(new QueueViewLog
                {
                    MessageId = entry.MessageId,
                    InsertionTime = entry.InsertedOn,
                    RawMessage = rawMessage
                });
            }

            return entryList;
        }

        public async Task ClearLogsAsync()
        {
            // Assuming you use Azure Queue Storage
            var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 32);
            while (messages.Value.Length > 0)
            {
                foreach (var msg in messages.Value)
                {
                    await _queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
                }
                messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 32);
            }
        }


    }
}
