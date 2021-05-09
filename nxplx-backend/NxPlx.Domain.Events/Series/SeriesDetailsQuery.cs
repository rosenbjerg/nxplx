using NxPlx.Application.Models.Series;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class SeriesDetailsQuery : IDomainQuery<SeriesDto?>
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