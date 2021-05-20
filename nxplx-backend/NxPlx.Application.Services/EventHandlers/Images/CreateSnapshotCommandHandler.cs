using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using FFMpegCore;
using NxPlx.Application.Events.Images;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers.Images
{
    public class CreateSnapshotCommandHandler : IApplicationEventHandler<CreateSnapshotCommand, string>
    {
        private readonly TempFileService _tempFileService;

        public CreateSnapshotCommandHandler(TempFileService tempFileService)
        {
            _tempFileService = tempFileService;
        }

        public async Task<string> Handle(CreateSnapshotCommand @event, CancellationToken cancellationToken = default)
        {
            var tempFile = _tempFileService.GetFilename("generated", ".png");
            var analysis = await FFProbe.AnalyseAsync(@event.InputVideoFilePath);
            await FFMpeg.SnapshotAsync(@event.InputVideoFilePath, tempFile, new Size(@event.Width, @event.Height), analysis!.Duration * @event.SeekPercentage);
            return tempFile;
        }
    }
}