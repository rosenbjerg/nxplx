using System.Collections.Generic;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Library
{
    public class ListLibrariesQuery : IDomainQuery<List<LibraryDto>>
    {
    }
}