namespace NxPlx.Application.Models.Events
{
    public class UpdateUserPasswordEvent : IEvent<bool>
    {
        public UpdateUserPasswordEvent(string oldPassword, string newPassword1, string newPassword2)
        {
            OldPassword = oldPassword;
            NewPassword1 = newPassword1;
            NewPassword2 = newPassword2;
        }

        public string OldPassword { get; }
        public string NewPassword1 { get; }
        public string NewPassword2 { get; }
    }
}