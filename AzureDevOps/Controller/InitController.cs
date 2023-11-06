using System.Net;
using System.Text;
using AzureDevOps.DAL.Interface;
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
        private readonly IJobRepository<Job> _repository;

        public InitController(ILoggerFactory loggerFactory, IJobRepository<Job> repo)
        {
            _logger     = loggerFactory.CreateLogger<InitController>();
            _repository = repo;
        }

        [Function("init")] 
        public async Task<OutPut> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Guid uuid           = Guid.NewGuid();
            Job job             = new Job(uuid.ToString(), 1);
            string serializeJob = JsonConvert.SerializeObject(job);

            var response = req.CreateResponse(HttpStatusCode.OK);
            var content  = new StringContent("started job with id: " + job.JobId, Encoding.UTF8, "text/plain");
            await content.CopyToAsync(response.Body);

            await _repository.GetOrCreateTableAsync("test");
            await _repository.CreateAsync(job);

            return new OutPut
            {
                Job = serializeJob,
                HttpResponse = response
            };
        }

        public class OutPut
        {
            [QueueOutput("imagequeue")]
            public string? Job { get; set; }
            public HttpResponseData? HttpResponse { get; set; }
        }
    }
}
