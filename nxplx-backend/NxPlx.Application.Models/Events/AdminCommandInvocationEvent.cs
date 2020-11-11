namespace NxPlx.Application.Models.Events
{
    public class AdminCommandInvocationEvent : IEvent<string?>
    {
        public AdminCommandInvocationEvent(string commandName, string[] arguments)
        {
            CommandName = commandName;
            Arguments = arguments;
        }

        public string CommandName { get; }
        public string[] Arguments { get; }
    }
}