namespace NxPlx.Models
{
    public interface IStillImageOwner : IImageOwner
    {
        public string StillPath { get; set; }
        public string StillBlurHash { get; set; }
    }
}