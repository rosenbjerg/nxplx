using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;

namespace NxPlx.Services.Database
{
    public class DatabaseContextManager
    {
        public void Register(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<MediaContext>()
                .AddDbContext<UserContext>()
                .BuildServiceProvider();
        }

        public async Task Initialize(ILoggingService logger)
        {
            await Initialize<MediaContext>(logger);
            await Initialize<UserContext>(logger);
        }

        private static async Task Initialize<TDbContext>(ILoggingService logger)
            where TDbContext : DbContext, new()
        {
            var databaseName = typeof(TDbContext).Name;
            await using var context = new TDbContext();
            
            var pendingMigrations = new List<string>(await context.Database.GetPendingMigrationsAsync());

            if (pendingMigrations.Any())
            {
                logger.Info("Applying {PendingMigrationCount} migrations to {DatabaseName}: {PendingMigrations}",
                    pendingMigrations.Count, databaseName, pendingMigrations);
            }
            
            await context.Database.MigrateAsync();
        }
    }
}