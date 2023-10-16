using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

            BlobClient blobClient = containerClient.GetBlobClient(GetFormattedBlobName(imageName));

            using (Stream stream = new MemoryStream(imageBytes))
            {
                // Upload the image to Azure Blob Storage
                await blobClient.UploadAsync(stream, true);
            }
        }

        public IEnumerable<string> ListBlobUrls()
        {
            List<string> blobUrls = new List<string>();

            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            foreach (BlobItem blobItem in containerClient.GetBlobs())
            {
                // Construct the URL for each blob
                Uri blobUri = containerClient.GetBlobClient(blobItem.Name).Uri;
                blobUrls.Add(blobUri.ToString());
            }

            return blobUrls;
        }

        private string GetFormattedBlobName(string imageName)
        {
            return imageName.Replace(" ", "_");
        }
    }
}
