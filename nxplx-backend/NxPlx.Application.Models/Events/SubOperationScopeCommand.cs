using Microsoft.Extensions.DependencyInjection;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Models.Events
{
    public class SubOperationScopeCommand : IApplicationCommand<IServiceScope>
    {
    }
}