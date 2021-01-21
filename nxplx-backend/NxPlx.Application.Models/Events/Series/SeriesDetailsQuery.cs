using NxPlx.Application.Models.Series;

namespace NxPlx.Application.Models.Events.Series
{
    public class SeriesDetailsQuery : IQuery<SeriesDto?>
    {
        public SeriesDetailsQuery(int id, int? season)
        {
            Id = id;
            Season = season;
        }
        
        public int Id { get; }
        public int? Season { get; }
    }
}