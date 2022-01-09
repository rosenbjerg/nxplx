
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Events.Images;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers.Images
{
    public abstract class SetImageCommandHandler<TImageOwner> : IApplicationEventHandler<SetImageCommand<TImageOwner>>
        where TImageOwner : IImageOwner
    {
        private readonly TempFileService _tempFileService;
        private readonly string _imageFolder;
        protected readonly IImageService ImageService;

        protected SetImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService, IImageService imageService)
        {
            _tempFileService = tempFileService;
            ImageService = imageService;
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
            await ImageService.ResizeImage(tempFile, width, height, Path.Combine(directory, filename));
        }



        public abstract Task Handle(SetImageCommand<TImageOwner> @event, CancellationToken cancellationToken = default);
    }
}