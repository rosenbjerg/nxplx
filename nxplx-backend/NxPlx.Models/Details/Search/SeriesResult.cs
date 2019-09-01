using System;
using System.Collections.Generic;

namespace NxPlx.Models.Details.Search
{
    public class SeriesResult : ResultBase
    {
        public DateTime? FirstAirDate { get; set; }
        public List<string> OriginCountry { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
    }
}