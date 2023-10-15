using AzureDevOps.Service.Interface;
using ImageMagick;

namespace AzureDevOps.Service
{
    public class GenerateImageOnTextService : IGenerateImageOnTextService
    {

        public byte[] AddTextToImage(byte[] imageBytes)
        {
            using MemoryStream imageStream = new MemoryStream(imageBytes);
            using MagickImage image        = new MagickImage(imageStream);

            image.Settings.FillColor     = MagickColors.Blue;
            image.Settings.BorderColor   = MagickColors.Black;
            image.Settings.FontWeight    = FontWeight.Bold;
            image.Settings.FontPointsize = 20;

            DrawableText text = new DrawableText(50, 100, "Hello, World");

            image.Draw(text);

            return image.ToByteArray();
        }
    }
}
