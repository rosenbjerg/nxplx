using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Sessions
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