using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Film
{
    public class RemoveUserCommand : IDomainCommand<bool>
    {
        public RemoveUserCommand(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }
}