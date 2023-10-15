using AzureDevOps.Model;
using AzureDevOps.Service.Interface;
using Newtonsoft.Json;

namespace AzureDevOps.Service
{
    public class DownloadWeatherApiService : IDownloadWeatherApiService
    {
        private const string BuienradarUrl = "https://data.buienradar.nl/2.0/feed/json";

        private string GetBuienradarDataUrl()
        {
            return BuienradarUrl;
        }

        public async Task<BuienradarData?> DownloadBuienradarDataAsync()
        {
            string buienradarUrl = GetBuienradarDataUrl();
            byte[] jsonData = await DownloadImageAsync(buienradarUrl);
            string json = System.Text.Encoding.UTF8.GetString(jsonData);
            return JsonConvert.DeserializeObject<BuienradarData>(json);
        }

        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(imageUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] data = await response.Content.ReadAsByteArrayAsync();
                        return data;
                    }
                    else
                    {
                        throw new Exception($"Failed to download data. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while downloading data.", ex);
                }
            }
        }
    }
}
