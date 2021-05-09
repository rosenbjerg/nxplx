using System;
using System.Threading.Tasks;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Sessions
{
    public class CreateSessionCommand : IDomainEvent<Task>
    {
        public CreateSessionCommand(int userId, string token, Session session, TimeSpan validity)
        {
            UserId = userId;
            Token = token;
            Session = session;
            Validity = validity;
        }

        public int UserId { get; }
        public string Token { get; }
        public Session Session { get; }
        public TimeSpan Validity { get; }
    }
}