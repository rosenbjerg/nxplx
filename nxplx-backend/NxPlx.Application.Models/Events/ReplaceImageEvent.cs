using System.IO;

namespace NxPlx.Application.Models.Events
{
    public class ReplaceImageEvent : IEvent<bool>
    {
        public ReplaceImageEvent(DetailsType detailsType, int detailsId, ImageType imageType, string imageExtension, Stream imageStream)
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