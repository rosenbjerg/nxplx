using System.IO;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Events
{
    public class ReplaceImageCommand : IApplicationCommand<bool>
    {
        public ReplaceImageCommand(DetailsType detailsType, int detailsId, ImageType imageType, string imageExtension, Stream imageStream)
        {
            DetailsType = detailsType;
            DetailsId = detailsId;
            ImageType = imageType;
            ImageExtension = imageExtension;
            ImageStream = imageStream;
        }

        public DetailsType DetailsType { get; }
        public int DetailsId { get; }
        public ImageType ImageType { get; }
        public string ImageExtension { get; }
        public Stream ImageStream { get; }
    }
}