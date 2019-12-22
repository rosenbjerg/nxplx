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
                .AddDbContext<NxplxContext>()
                .BuildServiceProvider();
        }

        public async Task Initialize(ILoggingService logger)
        {
            await Initialize<NxplxContext>(logger);
        }

        private static async Task Initialize<TDbContext>(ILoggingService logger)
            where TDbContext : DbContext, new()
        {
            var startTime = DateTime.UtcNow;
            var databaseName = typeof(TDbContext).Name;
            await using var context = new TDbContext();
            
            logger.Trace("Initializing and checking for migrations for {DatabaseName}", databaseName);
            var pendingMigrations = new List<string>(await context.Database.GetPendingMigrationsAsync());

            if (pendingMigrations.Any())
            {
                logger.Info("Applying {PendingMigrationCount} migrations to {DatabaseName}: {PendingMigrations}",
                    pendingMigrations.Count, databaseName, pendingMigrations);
            }
            
            await context.Database.MigrateAsync();
            
            var elapsed = (int) DateTime.UtcNow.Subtract(startTime).TotalSeconds;
            logger.Info("Initialized and applied {AppliedPendingMigrationCount} migrations to {DatabaseName} in {ElapsedSeconds} second(s)", pendingMigrations.Count, databaseName, elapsed);

        }
    }
}