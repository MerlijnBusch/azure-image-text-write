using AzureDevOps.Service.Interface;
using Microsoft.Extensions.Logging;

namespace AzureDevOps.Service
{
    public class DownloadImageService : IDownloadImageService
    {
        private const string ImageBaseUrl = "";
        private readonly ILogger<DownloadImageService> _logger;

        public DownloadImageService(ILogger<DownloadImageService> logger) {
            _logger = logger;
        }

        private string BuildImageUrl()
        {
            return "https://picsum.photos/500/900";
        }

        public async Task<byte[]> DownloadImageAsync()
        {
            string imageUrl = BuildImageUrl();

            using var httpClient = new HttpClient();
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(imageUrl);

                if (response.IsSuccessStatusCode)
                {
                    byte[] imageData = await response.Content.ReadAsByteArrayAsync();
                    return imageData;
                }
                else
                {
                    throw new Exception("Failed to download image");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while downloading the image.", ex);
            }
        }
    }
}
