using System.Collections.Generic;

namespace NxPlx.Integrations.TMDb.Models.Search
{
    public class SearchResult<TResult> where TResult : ResultBase
    {
        public List<TResult> results { get; set; }
    }
}