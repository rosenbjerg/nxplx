using System;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Infrastructure.Database;

namespace NxPlx.ApplicationHost.Api
{
    public static class JobDashboardExtension
    {
        public static void AddJobProcessing(this IServiceCollection services, ConnectionStrings connectionStrings)
        {
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Math.Max(Environment.ProcessorCount - 1, 2);
                options.Queues = JobQueueNames.All;
            });

            HangfireContext.EnsureCreated(connectionStrings.HangfirePgsql);
            var hangfireConfiguration = ConfigureHangfire(connectionStrings);
            services.AddHangfire(hangfireConfiguration);
        }
        
        public static void UseJobDashboard(this IApplicationBuilder app, string dashboardUrl, JobDashboardOptions jobDashboardOptions)
        {
            if (jobDashboardOptions.Enabled)
            {
                app.UseHangfireDashboard(dashboardUrl, new DashboardOptions
                {
                    Authorization = new[] { new IntegratedHangfireAuthentication() }
                });
            }
        }

        private static Action<IGlobalConfiguration> ConfigureHangfire(ConnectionStrings connectionStrings)
        {
            return hangfireConfiguration => hangfireConfiguration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(connectionStrings.HangfirePgsql);
        }
    }
}