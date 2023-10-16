using AzureDevOps.Service.Interface;
using ImageMagick;

namespace AzureDevOps.Service
{
    public class GenerateImageOnTextService : IGenerateImageOnTextService
    {

        public byte[] AddTextToImage(byte[] imageBytes, string t)
        {
            using MemoryStream imageStream = new MemoryStream(imageBytes);
            using MagickImage image        = new MagickImage(imageStream);

            image.Settings.FillColor     = MagickColors.Blue;
            image.Settings.BorderColor   = MagickColors.Black;
            image.Settings.FontWeight    = FontWeight.Bold;
            image.Settings.FontPointsize = 20;

            DrawableText text = new DrawableText(2, 10, t);

            image.Draw(text);

            return image.ToByteArray();
        }
    }
}
