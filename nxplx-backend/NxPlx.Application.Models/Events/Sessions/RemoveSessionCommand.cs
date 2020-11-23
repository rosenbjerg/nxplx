namespace NxPlx.Application.Models.Events.Sessions
{
    public class RemoveSessionCommand : ICommand
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