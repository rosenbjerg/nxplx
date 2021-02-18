using NxPlx.Infrastructure.Events.Events;
using NxPlx.Domain.Models;

namespace NxPlx.Application.Models.Events.Sessions
{

    public class SessionQuery : IDomainQuery<Session?>
    {
        public SessionQuery(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}