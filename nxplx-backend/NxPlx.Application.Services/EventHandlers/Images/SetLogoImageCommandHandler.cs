using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Events.Images;
using NxPlx.Domain.Models;

namespace NxPlx.Application.Services.EventHandlers.Images
{
    public class SetLogoImageCommandHandler : SetImageCommandHandler<ILogoImageOwner>
    {
        public SetLogoImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService, IImageService imageService)
            : base(folderOptions, tempFileService, imageService)
        {
        }

        public override async Task Handle(SetImageCommand<ILogoImageOwner> command, CancellationToken cancellationToken = default)
        {
            await CreateSizedCopy(command.InputFilepath, 120, 120, command.OutputFilename);
            if (command.ImageOwner.LogoPath != command.OutputFilename)
                DeleteOldImages(command.ImageOwner.LogoPath, 120);
            command.ImageOwner.LogoPath = command.OutputFilename;
            command.ImageOwner.LogoBlurHash = await ImageService.GenerateBlurhash(command.InputFilepath);
        }
    }
}