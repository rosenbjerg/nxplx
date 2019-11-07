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
            await using (var context = new MediaContext())
            {
                await Migrate(logger, context, nameof(MediaContext));
            }

            await using (var context = new UserContext())
            {
                await Migrate(logger, context, nameof(UserContext));
            }
        }

        private static async Task Migrate(ILoggingService logger, DbContext context, string databaseName)
        {
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