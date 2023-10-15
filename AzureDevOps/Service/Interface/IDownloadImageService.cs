using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Service.Interface
{
    public interface IDownloadImageService
    {
        public Task<byte[]> DownloadImageAsync();
    }
}
