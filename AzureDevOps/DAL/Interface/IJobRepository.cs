using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.DAL.Interface
{
    public interface IJobRepository<T> where T : TableEntity, new()
    {
        Task CreateAsync(T entity);
        public Task GetOrCreateTableAsync(string tableName);
        public Task<T?> FindByIdAsync(string partitionKey, string rowKey);
        public Task UpdateAsync(T entity);
    }
}
