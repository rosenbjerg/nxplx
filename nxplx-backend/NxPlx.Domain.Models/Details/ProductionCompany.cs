using System.Collections.Generic;
using NxPlx.Domain.Models.Database;

namespace NxPlx.Domain.Models.Details
{
    public class ProductionCompany : EntityBase, ILogoImageOwner
    {
        public string LogoPath { get; set; }
        public string LogoBlurHash { get; set; }
        public string Name { get; set; }
        public string OriginCountry { get; set; }

        public List<DbSeriesDetails> Series { get; set; }
        public List<DbFilmDetails> Film { get; set; }
    }
}