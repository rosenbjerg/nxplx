using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.User
{
    public class UpdateUserDetailsCommand : IDomainCommand<bool>
    {
        public UpdateUserDetailsCommand(string? email)
        {
            Email = email;
        }

        public string? Email { get; }
    }
}