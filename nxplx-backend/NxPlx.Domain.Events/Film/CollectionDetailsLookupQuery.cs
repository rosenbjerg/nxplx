using NxPlx.Application.Models.Film;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Film
{
    public class CollectionDetailsLookupQuery : IDomainQuery<MovieCollectionDto>
    {
        public CollectionDetailsLookupQuery(int collectionId)
        {
            CollectionId = collectionId;
        }

        public int CollectionId { get; }
    }
}