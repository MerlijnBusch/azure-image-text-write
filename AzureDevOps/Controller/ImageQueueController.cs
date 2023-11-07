using System;
using System.Text;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureDevOps.Model;
using AzureDevOps.Service;
using AzureDevOps.Service.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace AzureDevOps.Controller
{
    public class ImageQueueController
    {
        private readonly ILogger<ImageQueueController> _logger;
        private readonly IDownloadWeatherApiService _downloadWeatherApiService;

        public ImageQueueController(ILogger<ImageQueueController> logger, IDownloadWeatherApiService downloadWeatherApiService)
        {
            _logger = logger;
            _downloadWeatherApiService = downloadWeatherApiService;
        }

        [Function(nameof(ImageQueueController))]
        public async Task Run([QueueTrigger("imagequeue", Connection = "default")] QueueMessage message)
        {
            if (message == null || message.MessageText == null)
            {
                return;
            }

            Job job = JsonConvert.DeserializeObject<Job>(message.MessageText);

            if (job == null || job.JobId == null)
            {
                return;
            }

            BuienradarData? data = await _downloadWeatherApiService.DownloadBuienradarDataAsync();

            if (data == null || data.actual == null || data.actual.stationmeasurements == null)
            {
                return;
            }

            QueueClient queueClient = InitializeQueueClient();

            int totalItems = data.actual.stationmeasurements.Count; // Total number of items
            int currentItem = 1; // Counter for the current item

            foreach (var stationMeasurement in data.actual.stationmeasurements)
            {
                bool isLastItem = currentItem == totalItems;

                QueueData queueData = new(job.JobId, stationMeasurement, isLastItem);

                string serialize = JsonConvert.SerializeObject(queueData);

                _logger.LogInformation($"Station Name: {queueData}");

                var bytes = Encoding.UTF8.GetBytes(serialize);
                await queueClient.SendMessageAsync(Convert.ToBase64String(bytes));

                currentItem++;
            }
        }

        private QueueClient InitializeQueueClient()
        {
            var connectionString = "UseDevelopmentStorage=true"; // Connection string for the local Azure Storage Emulator
            var queueServiceClient = new QueueServiceClient(connectionString);
            var queueClient = queueServiceClient.GetQueueClient("imagequeuegenerate");

            if (!queueClient.Exists())
            {
                queueClient.Create();
            }

            return queueClient;
        }
    }
}
