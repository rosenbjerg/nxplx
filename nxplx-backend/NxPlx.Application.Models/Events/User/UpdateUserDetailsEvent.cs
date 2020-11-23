namespace NxPlx.Application.Models.Events.User
{
    public class UpdateUserDetailsCommand : ICommand<bool>
    {
        public UpdateUserDetailsCommand(string? email)
        {
            Email = email;
        }

        public string? Email { get; }
    }
}