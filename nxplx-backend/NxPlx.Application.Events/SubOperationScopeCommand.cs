using Microsoft.Extensions.DependencyInjection;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Events
{
    public class SubOperationScopeCommand : IApplicationCommand<IServiceScope>
    {
    }
}