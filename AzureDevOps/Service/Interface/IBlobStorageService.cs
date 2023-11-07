using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Service.Interface
{
    public interface IBlobStorageService
    {
        public Task UploadImageAsync(string containerName, byte[] imageBytes, string imageName);

        public IEnumerable<string> ListBlobUrls(string containerName);
    }
}
