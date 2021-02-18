using NxPlx.Application.Models.Film;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Film
{
    public class FilmDetailsLookupQuery : IDomainQuery<FilmDto?>
    {
        public FilmDetailsLookupQuery(int fileDetailsId)
        {
            FileDetailsId = fileDetailsId;
        }

        public int FileDetailsId { get; }
    }
}