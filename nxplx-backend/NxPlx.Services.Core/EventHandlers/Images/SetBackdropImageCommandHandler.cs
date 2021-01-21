using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Models.Events.Images;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Images
{
    public class SetBackdropImageCommandHandler : SetImageCommandHandlerBase<IBackdropImageOwner>
    {
        public SetBackdropImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService) : base(folderOptions, tempFileService)
        {
        }

        public override async Task Handle(SetImageCommand<IBackdropImageOwner> command, CancellationToken cancellationToken = default)
        {
            await CreateSizedCopy(command.InputFilepath, 1280, 1280, command.OutputFilename);
            if (command.ImageOwner.BackdropPath != command.OutputFilename)
                DeleteOldImages(command.ImageOwner.BackdropPath, 1280);
            command.ImageOwner.BackdropPath = command.OutputFilename;
            command.ImageOwner.BackdropBlurHash = await GenerateBlurhashFromThumbnail(command.InputFilepath);
        }
    }
}