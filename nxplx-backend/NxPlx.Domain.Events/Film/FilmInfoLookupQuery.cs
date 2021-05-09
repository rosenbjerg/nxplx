using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Film
{
    public class FilmInfoLookupQuery : IDomainQuery<InfoDto>
    {
        public FilmInfoLookupQuery(int fileId)
        {
            FileId = fileId;
        }

        public int FileId { get; }
    }
}