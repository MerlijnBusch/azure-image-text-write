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

        public BlobStorageService()
        {
            _connectionString = "UseDevelopmentStorage=true";
        }

        public async Task UploadImageAsync(string containerName, byte[] imageBytes, string imageName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName.Replace("-", ""));

            // Create the container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(GetFormattedBlobName(imageName));

            using (Stream stream = new MemoryStream(imageBytes))
            {
                // Upload the image to Azure Blob Storage
                await blobClient.UploadAsync(stream, true);
            }
        }

        public IEnumerable<string> ListBlobUrls(string containerName)
        {
            List<string> blobUrls = new List<string>();

            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName.Replace("-", ""));

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
