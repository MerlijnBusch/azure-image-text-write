using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Model
{
    public class Job : TableEntity
    {
        public string ?JobId { get; set; }
        public int ?Status { get; set; }

        public Job()
        {
        }

        public Job(string jobId, int status)
        {
            this.JobId = jobId;
            this.Status = status;

            PartitionKey = "Job_Status";
            RowKey = jobId;
        }
    }

    public enum JobStatus
    {
        Started = 1,
        Done = 2
    }
}
