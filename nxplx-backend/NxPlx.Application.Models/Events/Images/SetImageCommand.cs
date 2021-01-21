using NxPlx.Models;

namespace NxPlx.Application.Models.Events.Images
{
    public class SetImageCommand<TImageOwner> : ICommand
        where TImageOwner : IImageOwner
    {
        public SetImageCommand(TImageOwner imageOwner, string inputFilepath, string outputFilename)
        {
            ImageOwner = imageOwner;
            InputFilepath = inputFilepath;
            OutputFilename = outputFilename;
        }

        public TImageOwner ImageOwner { get; }
        public string InputFilepath { get; }
        public string OutputFilename { get; }
    }
}