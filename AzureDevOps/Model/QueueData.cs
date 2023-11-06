using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Model
{
    public class QueueData
    {
        public string? JobId { get; set; }
        public StationMeasurement Measurement { get; set; }
        public bool Finished { get; set; }

        public QueueData(string id, StationMeasurement measurement, bool final)
        {
            JobId = id;
            Measurement = measurement;
            Finished = final;
        }
    }
}
