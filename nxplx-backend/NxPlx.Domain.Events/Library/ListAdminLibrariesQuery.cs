using System.Collections.Generic;
using NxPlx.Application.Models.Jobs;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Library
{
    public class ListAdminLibrariesQuery : IDomainQuery<List<AdminLibraryDto>>
    {
    }
}