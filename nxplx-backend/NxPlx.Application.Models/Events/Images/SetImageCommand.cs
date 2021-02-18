using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;
using NxPlx.Domain.Models;

namespace NxPlx.Application.Models.Events.Images
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