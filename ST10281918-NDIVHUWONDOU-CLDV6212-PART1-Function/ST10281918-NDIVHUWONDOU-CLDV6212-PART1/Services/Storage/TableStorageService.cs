using Azure;
using Azure.Data.Tables;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage
{
    
    public class TableStorageService<T> where T : class, ITableEntity, new()
    {
        //private readonly TableClient _tableClient;

        //public TableStorageService(string storageAccount, string tableName)
        //{
        //    var serviceClient = new TableServiceClient(storageAccount);
        //    _tableClient = serviceClient.GetTableClient(tableName);
        //    _tableClient.CreateIfNotExists();
        //}
        //public async Task<List<Customer>> GetCustomersAsync()
        //{
        //    var customers = new List<Customer>();
        //    await foreach (var customer in _tableClient.QueryAsync<Customer>())
        //    {
        //        customers.Add(customer);
        //    }
        //    return customers;
        //}

        //public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
        //{
        //    try
        //    {
        //        var respones = await _tableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
        //        return respones.Value;
        //    }
        //    catch (RequestFailedException ex) when (ex.Status == 404)
        //    {
        //        return null;
        //    }
        //}

        //public async Task AddCustomerAsync(Customer customer)
        //{
        //    customer.PartitionKey = customer.PartitionKey;
        //    customer.RowKey = Guid.NewGuid().ToString();
        //    await _tableClient.AddEntityAsync(customer);
        //}
        //public async Task UpdateCustomerAsync(Customer customer)
        //{
        //    await _tableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);

        //}

        //public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        //{
        //    await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        //}

        //defined table client
        private readonly TableClient _tableClient;

        //initialise the constructor
        public TableStorageService(string storageConnectionString, string tableName)
        {
            var TableServiceClient = new TableServiceClient(storageConnectionString);
            _tableClient = TableServiceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        // Get all entities
        public async Task<List<T>> GetAllAsync()
        {
            var results = new List<T>();
            await foreach (var entity in _tableClient.QueryAsync<T>())
            {
                results.Add(entity);
            }
            return results;
        }

        // Get entity by PK + RK
        public async Task<T?> GetAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        // Insert
        public async Task AddAsync(T entity)
        {
            await _tableClient.AddEntityAsync(entity);
        }

        // Update
        public async Task UpdateAsync(T entity)
        {
            await _tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace);
        }

        // Delete
        public async Task DeleteAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}
