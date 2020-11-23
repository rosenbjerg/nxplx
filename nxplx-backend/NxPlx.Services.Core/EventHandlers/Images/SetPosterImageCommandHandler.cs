﻿using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Models.Events.Images;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Images
{
    public class SetPosterImageCommandHandler : SetImageCommandHandlerBase<IPosterImageOwner>
    {
        public SetPosterImageCommandHandler(FolderOptions folderOptions, TempFileService tempFileService) : base(folderOptions, tempFileService)
        {
        }

        public override async Task Handle(SetImageCommand<IPosterImageOwner> command, CancellationToken cancellationToken = default)
        {
            await CreateSizedCopy(command.InputFilepath, 190, 380, command.OutputFilename);
            await CreateSizedCopy(command.InputFilepath, 270, 540, command.OutputFilename);
            if (command.ImageOwner.PosterPath != command.OutputFilename)
                DeleteOldImages(command.ImageOwner.PosterPath, 190, 270);
            command.ImageOwner.PosterPath = command.OutputFilename;
            command.ImageOwner.PosterBlurHash = await GenerateBlurhashFromThumbnail(command.InputFilepath);
        }
    }
}