using System.Collections.Generic;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Library
{
    public class LibraryAccessQuery : IDomainQuery<List<int>>
    {
        public LibraryAccessQuery(int userId)
        {
            UserId = userId;
        }
        public int UserId { get; }
    }
}