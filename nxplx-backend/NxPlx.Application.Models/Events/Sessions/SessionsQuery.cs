namespace NxPlx.Application.Models.Events.Sessions
{
    public class SessionsQuery : IQuery<SessionDto[]>
    {
        public SessionsQuery(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }
    }
}