using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Models.Events.Images
{
    public class CreateSnapshotCommand : IApplicationCommand<string>
    {
        public CreateSnapshotCommand(string inputVideoFilePath, double seekPercentage, int width, int height)
        {
            InputVideoFilePath = inputVideoFilePath;
            SeekPercentage = seekPercentage;
            Width = width;
            Height = height;
        }
        public string InputVideoFilePath { get; set; } = null!;
        public double SeekPercentage { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}