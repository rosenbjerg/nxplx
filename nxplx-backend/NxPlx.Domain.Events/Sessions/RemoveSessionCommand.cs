using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Sessions
{
    public class RemoveSessionCommand : IDomainCommand<Task>
    {
        public int UserId { get; }
        public string Token { get; }

        public RemoveSessionCommand(int userId, string token)
        {
            UserId = userId;
            Token = token;
        }
    }
}