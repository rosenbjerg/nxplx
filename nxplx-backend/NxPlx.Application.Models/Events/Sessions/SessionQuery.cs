using NxPlx.Models;

namespace NxPlx.Application.Models.Events.Sessions
{
    public class SessionQuery : IQuery<Session?>
    {
        public SessionQuery(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}