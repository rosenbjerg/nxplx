using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Events.Images;
using NxPlx.Domain.Models;

namespace NxPlx.Application.Services.EventHandlers.Images
{
    public class SetPosterImageCommandHandler : SetImageCommandHandler<IPosterImageOwner>
    {
        public SetPosterImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService, IImageService imageService) 
            : base(folderOptions, tempFileService, imageService)
        {
        }

        public override async Task Handle(SetImageCommand<IPosterImageOwner> command, CancellationToken cancellationToken = default)
        {
            await CreateSizedCopy(command.InputFilepath, 190, 380, command.OutputFilename);
            await CreateSizedCopy(command.InputFilepath, 270, 540, command.OutputFilename);
            if (command.ImageOwner.PosterPath != command.OutputFilename)
                DeleteOldImages(command.ImageOwner.PosterPath, 190, 270);
            command.ImageOwner.PosterPath = command.OutputFilename;
            command.ImageOwner.PosterBlurHash = await ImageService.GenerateBlurhash(command.InputFilepath);
        }
    }
}