namespace NxPlx.Application.Models.Events.Series
{
    public class NextEpisodeByFileIdQuery : IQuery<NextEpisodeDto?>
    {
        public NextEpisodeByFileIdQuery(int fileId, string mode)
        {
            FileId = fileId;
            Mode = mode;
        }

        public int FileId { get; }
        public string Mode { get; }
    }
}