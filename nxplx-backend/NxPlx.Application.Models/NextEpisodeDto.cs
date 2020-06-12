namespace NxPlx.Application.Models
{
    public class NextEpisodeDto : IDto
    {
        public int fid { get; set; }
        public string title { get; set; } = null!;
        public string poster { get; set; } = null!;
    }
}