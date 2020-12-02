using System.Collections.Generic;
using NxPlx.Models.Database;

namespace NxPlx.Models.Details.Series
{
    public class Creator : EntityBase
    {
        public string Name { get; set; }
        public List<DbSeriesDetails> CreatedSeries { get; set; }
    }
}