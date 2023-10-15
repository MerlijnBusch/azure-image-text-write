using System.Net;
using System.Text;
using AzureDevOps.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureDevOps.Controller
{
    public class InitController
    {
        private readonly ILogger _logger;

        public InitController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<InitController>();
        }

        [Function("init")]
        public async Task<OutPut> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Image image           = new("test");
            string serializeImage = JsonConvert.SerializeObject(image);

            var response = req.CreateResponse(HttpStatusCode.OK);
            var content  = new StringContent("started", Encoding.UTF8, "text/plain");
            await content.CopyToAsync(response.Body);

            return new OutPut
            {
                Image = serializeImage,
                HttpResponse = response
            };
        }

        public class OutPut
        {
            [QueueOutput("imagequeue")]
            public string? Image { get; set; }
            public HttpResponseData? HttpResponse { get; set; }
        }
    }
}
