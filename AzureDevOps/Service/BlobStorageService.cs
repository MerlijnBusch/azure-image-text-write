using Azure.Storage.Blobs;
using AzureDevOps.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Service
{
    public class BlobStorageService : IBlobStorageService
    {
        private string _connectionString; // UseDevelopmentStorage=true for local Azure Storage Emulator
        private string _containerName;   // The name of the container

        public BlobStorageService()
        {
            _connectionString = "UseDevelopmentStorage=true";
            _containerName = "test";
        }

        public async Task UploadImageAsync(byte[] imageBytes, string imageName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Create the container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(imageName);

            using (Stream stream = new MemoryStream(imageBytes))
            {
                // Upload the image to Azure Blob Storage
                await blobClient.UploadAsync(stream, true);
            }
        }
    }
}
