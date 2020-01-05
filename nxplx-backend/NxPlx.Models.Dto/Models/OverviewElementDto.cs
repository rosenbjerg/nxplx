using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models
{
    public class OverviewElementDto : IDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string poster { get; set; }
        public string kind { get; set; }
        public int year { get; set; }
        public List<int> genres { get; set; }
    }
}