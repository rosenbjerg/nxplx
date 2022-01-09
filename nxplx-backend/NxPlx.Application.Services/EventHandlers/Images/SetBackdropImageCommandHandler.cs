using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Events.Images;
using NxPlx.Domain.Models;

namespace NxPlx.Application.Services.EventHandlers.Images
{
    public class SetBackdropImageCommandHandler : SetImageCommandHandler<IBackdropImageOwner>
    {
        public SetBackdropImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService, IImageService imageService)
            : base(folderOptions, tempFileService, imageService)
        {
        }

        public override async Task Handle(SetImageCommand<IBackdropImageOwner> command, CancellationToken cancellationToken = default)
        {
            await CreateSizedCopy(command.InputFilepath, 1280, 1280, command.OutputFilename);
            if (command.ImageOwner.BackdropPath != command.OutputFilename)
                DeleteOldImages(command.ImageOwner.BackdropPath, 1280);
            command.ImageOwner.BackdropPath = command.OutputFilename;
            command.ImageOwner.BackdropBlurHash = await ImageService.GenerateBlurhash(command.InputFilepath);
        }
    }
}