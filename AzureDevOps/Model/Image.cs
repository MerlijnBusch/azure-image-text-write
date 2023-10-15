using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOps.Model
{
    public class Image
    {
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }

        public Image(string imageName)
        {
            ImageName = imageName;
            ImageUrl  = "www.localhost.com/" + imageName;
        }
    }
}
