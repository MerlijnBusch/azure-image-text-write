using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using AzureDevOps.DAL.Interface;

namespace AzureDevOps.DAL
{
    public class JobRepository<T> : IJobRepository<T> where T : TableEntity, new()
    {
        private readonly ILogger _logger;
        private readonly CloudStorageAccount _storageAccount;
        protected CloudTableClient _tableClient;
        protected CloudTable? _table = null;

        public JobRepository(ILoggerFactory loggerFactory) : base()
        {
            _logger = loggerFactory.CreateLogger<JobRepository<T>>();
            _storageAccount = CloudStorageAccount.Parse(this.GetDatabaseUrlFromConfigurations());
            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        public async Task GetOrCreateTableAsync(string tableName)
        {
            if (_tableClient == null)
            {
                throw new InvalidOperationException("Table client is not initialized.");
            }

            // Get a reference to the table.
            _table = _tableClient.GetTableReference(tableName);

            // Check if the table exists and create it if not.
            if (!await _table.ExistsAsync())
            {
                await _table.CreateAsync();
            }
        }

        public async Task CreateAsync(T entity)
        {
            _logger.LogInformation("create async");

            if (_table == null)
            {
                throw new InvalidOperationException();
            }

            _logger.LogInformation("create async after");

            try
            {
                TableOperation insertOperation = TableOperation.Insert(entity);
                await _table.ExecuteAsync(insertOperation);
            }
            catch (StorageException ex)
            {
                _logger.LogInformation("Error: " + ex.Message);
                throw; // Rethrow the exception to propagate it if needed.
            }
        }

        public async Task<T?> FindByIdAsync(string partitionKey, string rowKey)
        {
            if (_table == null)
            {
                throw new InvalidOperationException("Table reference is not initialized.");
            }

            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                TableResult result = await _table.ExecuteAsync(retrieveOperation);

                if (result.Result is T entity)
                {
                    return entity;
                }
                else
                {
                    return null; // Entity not found
                }
            }
            catch (StorageException ex)
            {
                _logger.LogError("Error while retrieving entity: " + ex.Message);
                throw; // Rethrow the exception to propagate it if needed.
            }
        }

        public async Task UpdateAsync(T entity)
        {
            if (_table == null)
            {
                throw new InvalidOperationException("Table reference is not initialized.");
            }

            try
            {
                TableOperation replaceOperation = TableOperation.Replace(entity);
                await _table.ExecuteAsync(replaceOperation);
            }
            catch (StorageException ex)
            {
                _logger.LogError("Error while updating entity: " + ex.Message);
                throw; // Rethrow the exception to propagate it if needed.
            }
        }


        private string GetDatabaseUrlFromConfigurations()
        {
            string? url = "UseDevelopmentStorage=true";

            if (url == null)
            {
                throw new Exception();
            }

            return url;
        }
    }
}
