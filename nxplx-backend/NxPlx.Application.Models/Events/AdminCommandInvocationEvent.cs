namespace NxPlx.Application.Models.Events
{
    public class AdminCommandInvocationCommand : ICommand<string?>
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