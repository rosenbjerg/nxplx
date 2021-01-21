using System;
using NxPlx.Models;

namespace NxPlx.Application.Models.Events.Sessions
{
    public class CreateSessionCommand : ICommand
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