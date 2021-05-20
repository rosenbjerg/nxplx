using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events
{
    public class AdminCommandInvocationCommand : IDomainCommand<string?>
    {
        public AdminCommandInvocationCommand(string commandName, string[] arguments)
        {
            CommandName = commandName;
            Arguments = arguments;
        }

        public string CommandName { get; }
        public string[] Arguments { get; }
    }
}