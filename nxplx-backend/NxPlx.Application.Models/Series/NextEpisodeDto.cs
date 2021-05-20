namespace NxPlx.Application.Models.Series
{
    public class NextEpisodeDto : IDto
    {
        public int Fid { get; set; }
        public string Title { get; set; } = null!;
    }
}