namespace NxPlx.Domain.Models
{
    public interface ILogoImageOwner : IImageOwner
    {
        public string LogoPath { get; set; }
        public string LogoBlurHash { get; set; }
    }
}