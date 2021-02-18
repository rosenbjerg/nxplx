using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Models.Events.Sessions
{
    public class SessionsQuery : IDomainQuery<SessionDto[]>
    {
        public SessionsQuery(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }
    }
}