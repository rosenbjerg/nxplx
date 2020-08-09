namespace NxPlx.Models.Details.Series
{
    public class Network : EntityBase, ILogoImageOwner
    {
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string LogoBlurHash { get; set; }
        public string OriginCountry { get; set; }
    }
}