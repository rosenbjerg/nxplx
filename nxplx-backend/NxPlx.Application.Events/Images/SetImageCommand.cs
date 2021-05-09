using System.Threading.Tasks;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Events.Images
{
    public class SetImageCommand<TImageOwner> : IApplicationCommand<Task>
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