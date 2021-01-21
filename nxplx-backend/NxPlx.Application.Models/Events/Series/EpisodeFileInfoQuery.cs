namespace NxPlx.Application.Models.Events.Series
{
    public class EpisodeFileInfoQuery : IQuery<InfoDto?>
    {
        public EpisodeFileInfoQuery(int id)
        {
            Id = id;
        }
        
        public int Id { get; }
    }
}