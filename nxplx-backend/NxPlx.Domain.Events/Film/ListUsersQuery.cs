using System.Collections.Generic;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Film
{
    public class ListUsersQuery : IDomainQuery<IEnumerable<UserDto>>
    {
    }
}