using AzureDevOps.DAL;
using AzureDevOps.DAL.Interface;
using AzureDevOps.Model;
using AzureDevOps.Service;
using AzureDevOps.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(configurationBuilder =>
    {
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddTransient<IDownloadImageService,       DownloadImageService>();
        services.AddTransient<IDownloadWeatherApiService,  DownloadWeatherApiService>();
        services.AddTransient<IGenerateImageOnTextService, GenerateImageOnTextService>();
        services.AddTransient<IBlobStorageService,         BlobStorageService>();
        services.AddTransient<IJobRepository<Job>,         JobRepository<Job>>();
    })
    .Build();

host.Run();
