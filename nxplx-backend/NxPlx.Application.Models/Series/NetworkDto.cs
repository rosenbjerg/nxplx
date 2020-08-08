namespace NxPlx.Application.Models.Series
{
    public class NetworkDto : IDto
    {
        public string Name { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string OriginCountry { get; set; } = null!;
        public string LogoBlurHash { get; set; } = null!;
    }
}