using System.Collections.Generic;
using NxPlx.Domain.Models.Database;

namespace NxPlx.Domain.Models.Details.Series
{
    public class Creator : EntityBase
    {
        public string Name { get; set; }
        public List<DbSeriesDetails> Series { get; set; }
    }
}