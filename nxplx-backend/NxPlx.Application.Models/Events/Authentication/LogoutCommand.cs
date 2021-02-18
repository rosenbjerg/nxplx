using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Models.Events.Authentication
{
    public class LogoutCommand : IApplicationEvent<bool>
    {
    }
}