using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public class OverviewElementDto : IDto
    {
        public int id { get; set; }
        public string title { get; set; } = null!;
        public string poster { get; set; } = null!;
        public string kind { get; set; } = null!;
        public int year { get; set; }
        public List<int> genres { get; set; } = null!;
    }
}