using System.Collections.Generic;

namespace NxPlx.Integrations.OMDb.Models
{
    public class SearchResult
    {
        public List<Result> Search { get; set; }
        public string totalResults { get; set; }
        public string Response { get; set; }
    }
}