namespace NxPlx.Application.Models.Events
{
    public class UpdateUserDetailsEvent : IEvent<bool>
    {
        public UpdateUserDetailsEvent(string? email)
        {
            Email = email;
        }

        public string? Email { get; }
    }
}