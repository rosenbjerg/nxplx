using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Sessions
{
    public class ClearSessionsCommand : IDomainCommand<Task>
    {
        public int UserId { get; }

        public ClearSessionsCommand(int userId)
        {
            UserId = userId;
        }
    }
}