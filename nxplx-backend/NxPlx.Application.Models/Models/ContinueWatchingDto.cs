using System;

namespace NxPlx.Models.Dto.Models
{
    public class ContinueWatchingDto : IDto
    {
        public int fileId { get; set; }
        public string title { get; set; }
        public string poster { get; set; }
        public string kind { get; set; }
        public DateTime watched { get; set; }
        public double progress { get; set; }
    }
}