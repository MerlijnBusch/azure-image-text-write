using System;
using Azure.Storage.Queues.Models;
using AzureDevOps.Service.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureDevOps.Controller
{
    public class GenerateImageController
    {
        private readonly ILogger<GenerateImageController> _logger;
        private readonly IDownloadImageService _downloadImageService;
        private readonly IGenerateImageOnTextService _generateImageOnTextService;
        private readonly IBlobStorageService _blobStorageService;

        public GenerateImageController(
            ILogger<GenerateImageController> logger,
            IDownloadImageService downloadImageService,
            IGenerateImageOnTextService generateImageOnTextService,
            IBlobStorageService blobStorageService
        ) {
            _logger                     = logger;
            _downloadImageService       = downloadImageService;
            _generateImageOnTextService = generateImageOnTextService;
            _blobStorageService         = blobStorageService;
        }

        [Function(nameof(GenerateImageController))]
        public async Task Run([QueueTrigger("imagequeuegenerate", Connection = "default")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            byte[] image = await _downloadImageService.DownloadImageAsync();

            _logger.LogInformation(image.Length.ToString());

            byte[] image2 = _generateImageOnTextService.AddTextToImage(image);

            await _blobStorageService.UploadImageAsync(image2, "some text");


            try
            {
                string filePath = "C:\\Users\\merli\\source\\repos\\AzureDevOps\\AzureDevOps\\Controller\\image.jpg"; // Update the file path as needed

                // Write the byte array to the file
                System.IO.File.WriteAllBytes(filePath, image2);

                // After this, you can open the saved image file using your preferred image viewer to verify if it downloaded correctly.
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the file writing process
                Console.WriteLine($"An error occurred while saving the image: {ex.Message}");
            }
        }
    }
}
