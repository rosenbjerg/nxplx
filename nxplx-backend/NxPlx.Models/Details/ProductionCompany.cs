namespace NxPlx.Models.Details
{
    public class ProductionCompany : EntityBase, ILogoImageOwner
    {
        public string LogoPath { get; set; }
        public string LogoBlurHash { get; set; }
        public string Name { get; set; }
        public string OriginCountry { get; set; }
    }
}