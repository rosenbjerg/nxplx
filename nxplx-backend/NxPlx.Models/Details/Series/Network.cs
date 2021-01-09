using System.Collections.Generic;
using NxPlx.Models.Database;

namespace NxPlx.Models.Details.Series
{
    public class Network : EntityBase, ILogoImageOwner
    {
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string LogoBlurHash { get; set; }
        public string OriginCountry { get; set; }
        public List<DbSeriesDetails> Series { get; set; }
    }
}