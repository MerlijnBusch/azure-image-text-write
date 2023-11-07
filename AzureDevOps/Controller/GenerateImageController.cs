using Azure.Storage.Queues.Models;
using AzureDevOps.DAL.Interface;
using AzureDevOps.Model;
using AzureDevOps.Service.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureDevOps.Controller
{
    public class GenerateImageController
    {
        private readonly ILogger<GenerateImageController> _logger;
        private readonly IDownloadImageService _downloadImageService;
        private readonly IGenerateImageOnTextService _generateImageOnTextService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IJobRepository<Job> _jobRepository;

        public GenerateImageController(
            ILogger<GenerateImageController> logger,
            IDownloadImageService downloadImageService,
            IGenerateImageOnTextService generateImageOnTextService,
            IBlobStorageService blobStorageService,
            IJobRepository<Job> jobRepository
        ) {
            _logger                     = logger;
            _downloadImageService       = downloadImageService;
            _generateImageOnTextService = generateImageOnTextService;
            _blobStorageService         = blobStorageService;
            _jobRepository              = jobRepository;
        }

        [Function(nameof(GenerateImageController))]
        public async Task Run([QueueTrigger("imagequeuegenerate", Connection = "default")] QueueMessage message)
        {
          //  _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            if(message.MessageText == null || message == null) {
                return;
            }

            QueueData queueData = JsonConvert.DeserializeObject<QueueData>(message.MessageText);

            StationMeasurement measurement = queueData.Measurement;

            string formattedMeasurementInfo = $@"
                Station: {measurement.stationname}
                Location: Latitude {measurement.lat}, Longitude {measurement.lon}
                Timestamp: {measurement.timestamp}
                Weather Description: {measurement.weatherdescription}
                Temperature: {measurement.temperature}°C
                Feel Temperature: {measurement.feeltemperature}°C
                Humidity: {measurement.humidity}%
                Wind: {measurement.winddirection} at {measurement.windspeed} m/s
                Precipitation: {measurement.precipitation} mm
                Sun Power: {measurement.sunpower} W/m²
                Rainfall (Last 24 Hours): {measurement.rainFallLast24Hour} mm
                Rainfall (Last Hour): {measurement.rainFallLastHour} mm
            ";

            byte[] image = await _downloadImageService.DownloadImageAsync();

          //  _logger.LogInformation(image.Length.ToString());

            byte[] image2 = _generateImageOnTextService.AddTextToImage(image, formattedMeasurementInfo);

            await _blobStorageService.UploadImageAsync(queueData.JobId, image2, measurement.stationname);

            if (queueData.Finished)
            {

                _logger.LogInformation("upload");

                await _jobRepository.GetOrCreateTableAsync("test");

                Job job = await _jobRepository.FindByIdAsync("Job_Status", queueData.JobId);
                job.Status = 2;

                await _jobRepository.UpdateAsync(job);
            }
        }
    }
}
