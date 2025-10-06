using ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services
{
    public class OrderService
    {
        private readonly TableStorageService<Order> _tableService;

        public OrderService(string connectionString, string tableName)
        {
            _tableService = new TableStorageService<Order>(connectionString, tableName);
        }

        public Task<List<Order>> GetAllOrdersAsync()
            => _tableService.GetAllAsync();

        public Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
            => _tableService.GetAsync(partitionKey, rowKey);

        public Task AddOrderAsync(Order order)
            => _tableService.AddAsync(order);


        public Task UpdateOrderAsync(Order order)

            => _tableService.UpdateAsync(order);

        public Task DeleteOrderAsync(string partitionKey, string rowKey)
            => _tableService.DeleteAsync(partitionKey, rowKey);
    }
}

