using System.Collections.Generic;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events
{
    public class MediaOverviewQuery : IDomainQuery<IEnumerable<OverviewElementDto>>
    {
    }
}