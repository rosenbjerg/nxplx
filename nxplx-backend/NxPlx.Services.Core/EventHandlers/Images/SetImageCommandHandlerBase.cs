using System.Drawing;
using System.Drawing.Blurhash;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Instances;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Models.Events.Images;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Images
{
    public abstract class SetImageCommandHandlerBase<TImageOwner> : IEventHandler<SetImageCommand<TImageOwner>>
        where TImageOwner : IImageOwner
    {
        private readonly TempFileService _tempFileService;
        private readonly string _imageFolder;

        protected SetImageCommandHandlerBase(FolderOptions folderOptions, TempFileService tempFileService)
        {
            _tempFileService = tempFileService;
            _imageFolder = folderOptions.Images;
        }
        
        protected void DeleteOldImages(string oldFilePath, params int[] sizes)
        {
            if (string.IsNullOrEmpty(oldFilePath)) return;
            foreach (var size in sizes)
            {
                var oldImagePath = Path.Combine(_imageFolder, $"w{size}", oldFilePath);
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }
        }
        protected async Task CreateSizedCopy(string tempFile, int width, int height, string filename)
        {
            var directory = Path.Combine(_imageFolder, $"w{width}");
            Directory.CreateDirectory(directory);
            await ResizeImage(tempFile, width, height, Path.Combine(directory, filename));
        }

        protected async Task<string> GenerateBlurhashFromThumbnail(string tempFile)
        {
            var tempMiniature = _tempFileService.GetFilename("thumb", ".jpg");
            await ResizeImage(tempFile, 90, 90, tempMiniature);
            
            var encoder = new Encoder();
            await using var imageStream = System.IO.File.OpenRead(tempMiniature);
            using var image = Image.FromStream(imageStream);
            if (image.Width > image.Height)
                return encoder.Encode(image, 4, 3);
            else
                return encoder.Encode(image, 3, 4);
        }
        
        private static async Task ResizeImage(string imagePath, int width, int height, string outputPath)
        {
            var imageExtension = Path.GetExtension(outputPath);
            var arg = $"\"{imagePath}\" -resize {height}x{width} -strip -trim -quality 80";
            if (imageExtension == ".png")
                arg += "-format PNG24";
            else if (imageExtension == ".jpg" || imageExtension == ".jpeg") 
                arg += " -interlace plane -format JPG";

            arg += $" \"{outputPath}\"";
            await Instance.FinishAsync("magick", arg);
        }

        public abstract Task Handle(SetImageCommand<TImageOwner> @event, CancellationToken cancellationToken = default);
    }
}