using System.Collections.Generic;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Events
{
    public class ListUsersQuery : IApplicationQuery<IEnumerable<UserDto>>
    {
    }
}