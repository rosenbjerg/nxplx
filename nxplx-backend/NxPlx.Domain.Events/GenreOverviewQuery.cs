using System.Collections.Generic;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events
{
    public class GenreOverviewQuery : IDomainQuery<IEnumerable<GenreDto>>
    {
    }
}