using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Events;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Extensions;

public static class DatabaseWebApplicationExtensions
{
    public static async Task InitializeDatabase(this IHost host, CancellationToken cancellationToken)
    {
        using var scope = host.Services.CreateScope();

        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var hangfireContext = scope.ServiceProvider.GetRequiredService<HangfireContext>();
        
        await databaseContext.Database.MigrateAsync(cancellationToken);
        await hangfireContext.Database.EnsureCreatedAsync(cancellationToken);

                
        var eventDispatcher = scope.ServiceProvider.GetRequiredService<IApplicationEventDispatcher>();
        await eventDispatcher.Dispatch(new CreateAdminCommand());
    }

}