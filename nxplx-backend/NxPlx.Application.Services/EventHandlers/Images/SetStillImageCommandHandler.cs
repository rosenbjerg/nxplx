using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Models.Events.Images;
using NxPlx.Domain.Models;

namespace NxPlx.Application.Services.EventHandlers.Images
{
    public class SetStillImageCommandHandler : SetImageCommandHandler<IStillImageOwner>
    {
        public SetStillImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService) : base(folderOptions, tempFileService)
        {
        }

        public override async Task Handle(SetImageCommand<IStillImageOwner> command, CancellationToken cancellationToken = default)
        {
            await CreateSizedCopy(command.InputFilepath, 260, 200, command.OutputFilename);
            if (command.ImageOwner.StillPath != command.OutputFilename)
                DeleteOldImages(command.ImageOwner.StillPath, 260);
            command.ImageOwner.StillPath = command.OutputFilename;
            command.ImageOwner.StillBlurHash = await GenerateBlurhashFromThumbnail(command.InputFilepath);
        }
    }
}