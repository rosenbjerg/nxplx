using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BlurHashSharp.Drawing;
using Instances;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Models.Events.Images;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers.Images
{
    public abstract class SetImageCommandHandler<TImageOwner> : IApplicationEventHandler<SetImageCommand<TImageOwner>>
        where TImageOwner : IImageOwner
    {
        private readonly TempFileService _tempFileService;
        private readonly string _imageFolder;

        protected SetImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService)
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
                if (File.Exists(oldImagePath))
                    File.Delete(oldImagePath);
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

            var isPortrait = await IsPortrait(tempMiniature);
            if (isPortrait)
                return BlurHashEncoder.Encode(3, 4, tempMiniature);
            else
                return BlurHashEncoder.Encode(4, 3, tempMiniature);
        }

        private async Task<bool> IsPortrait(string imagePath)
        {
            await using var imageStream = File.OpenRead(imagePath);
            using var image = Image.FromStream(imageStream);
            return image.Height > image.Width;
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