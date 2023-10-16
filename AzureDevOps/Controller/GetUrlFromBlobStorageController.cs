using System.Net;
using AzureDevOps.Service.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureDevOps.Controller
{
    public class GetUrlFromBlobStorageController
    {
        private readonly ILogger _logger;
        private readonly IBlobStorageService _blobStorageService;

        public GetUrlFromBlobStorageController(ILoggerFactory loggerFactory, IBlobStorageService blobStorageService)
        {
            _logger             = loggerFactory.CreateLogger<GetUrlFromBlobStorageController>();
            _blobStorageService = blobStorageService;   
        }

        [Function("blobs")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            List<string> blobUrls = (List<string>)_blobStorageService.ListBlobUrls();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            // Serialize the blobUrls list to JSON and set it as the response content
            string jsonBlobUrls = JsonConvert.SerializeObject(blobUrls);
            response.WriteString(jsonBlobUrls);

            return response;
        }
    }
}
