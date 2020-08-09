namespace NxPlx.Models
{
    public interface IBackdropImageOwner
    {
        public string BackdropPath { get; set; }
        public string BackdropBlurHash { get; set; }
    }
}