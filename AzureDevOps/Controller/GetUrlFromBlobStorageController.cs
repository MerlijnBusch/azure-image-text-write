using System.Net;
using System.Text;
using AzureDevOps.DAL.Interface;
using AzureDevOps.Model;
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
        private readonly IJobRepository<Job> _jobRepository;

        public GetUrlFromBlobStorageController(
            ILoggerFactory loggerFactory, 
            IBlobStorageService blobStorageService, 
            IJobRepository<Job> jobRepository
        ) {
            _logger             = loggerFactory.CreateLogger<GetUrlFromBlobStorageController>();
            _blobStorageService = blobStorageService;
            _jobRepository      = jobRepository;   
        }

        [Function("blobs")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("test");

            if (req.Method != "POST")
            {
                return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }

            try
            {
                // Read the JSON data from the request
                var requestBody = await req.ReadAsStringAsync();
                RequestData requestData = JsonConvert.DeserializeObject<RequestData>(requestBody);

                _logger.LogInformation(requestBody);

                // Validate the job_id as a UUID
                if (!Guid.TryParse(requestData.job, out _))
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }

                string uuid = requestData.job;

                await _jobRepository.GetOrCreateTableAsync("test");
                Job job = await _jobRepository.FindByIdAsync("Job_Status", uuid);

                if (job.Status == 1)
                {
                    var res = req.CreateResponse(HttpStatusCode.OK);
                    var content = new StringContent("started job with id: " + job.JobId + " is still beeing processed", Encoding.UTF8, "text/plain");
                    await content.CopyToAsync(res.Body);

                    return res;
                }

                List<string> blobUrls = (List<string>)_blobStorageService.ListBlobUrls(uuid);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                // Serialize the blobUrls list to JSON and set it as the response content
                string jsonBlobUrls = JsonConvert.SerializeObject(blobUrls);
                response.WriteString(jsonBlobUrls);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing the request.");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private class RequestData
        {
            public string? job { get; set; }
        }
    }
}
