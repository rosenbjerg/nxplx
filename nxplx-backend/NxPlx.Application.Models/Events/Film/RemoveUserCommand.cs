namespace NxPlx.Application.Models.Events.Film
{
    public class RemoveUserCommand : ICommand<bool>
    {
        public RemoveUserCommand(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }
}