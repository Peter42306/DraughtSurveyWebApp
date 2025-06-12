using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using DraughtSurveyWebApp.Interfaces;

namespace DraughtSurveyWebApp.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public async Task<string> SaveImageAsync(IFormFile uploadedFile)
        {
            if(uploadedFile == null || uploadedFile.Length == 0)
            {
                return string.Empty;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "image");
            Directory.CreateDirectory(uploadsFolder);

            var newFileNameGenerated = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
            var newFilePath = Path.Combine(uploadsFolder, newFileNameGenerated);

            using (var image = await Image.LoadAsync(uploadedFile.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(600, 600),
                    Mode = ResizeMode.Max
                }));

                await image.SaveAsJpegAsync(newFilePath);
            }

            return newFileNameGenerated;

        }

        public void DeleteImage(string fileName)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "image");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
    }
}
