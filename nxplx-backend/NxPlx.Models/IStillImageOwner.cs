namespace NxPlx.Models
{
    public interface IStillImageOwner
    {
        public string StillPath { get; set; }
        public string StillBlurHash { get; set; }
    }
}