using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Events.Authentication
{
    public class LogoutCommand : IApplicationEvent<bool>
    {
    }
}