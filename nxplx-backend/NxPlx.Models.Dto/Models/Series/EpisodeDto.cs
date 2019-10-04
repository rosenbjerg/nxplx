using System;

namespace NxPlx.Models.Dto.Models.Series
{
    public class EpisodeDto
    {
        public DateTime? airDate { get; set; }
        public int number { get; set; }
        public string name { get; set; }
        public string still { get; set; }
        public int fileId { get; set; }
    }
}