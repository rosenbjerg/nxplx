namespace NxPlx.Application.Models
{
    public class NextEpisodeDto : IDto
    {
        public int Fid { get; set; }
        public string Title { get; set; } = null!;
        public string Poster { get; set; } = null!;
    }
}