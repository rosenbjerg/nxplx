using System;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Infrastructure.Database;

namespace NxPlx.ApplicationHost.Api
{
    public static class JobDashboardExtension
    {
        public static void AddJobProcessing(this IServiceCollection services, IServiceProvider serviceProvider)
        {
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Math.Max(Environment.ProcessorCount - 1, 2);
                options.Queues = JobQueueNames.All;
            });

            var connectionStrings = serviceProvider.GetRequiredService<ConnectionStrings>();
            HangfireContext.EnsureCreated(connectionStrings.HangfirePgsql);
            var hangfireConfiguration = ConfigureHangfire(connectionStrings);
            hangfireConfiguration(GlobalConfiguration.Configuration);
            services.AddHangfire(hangfireConfiguration);
        }
        
        public static void ConfigureJobDashboard(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var jobDashboardOptions = serviceProvider.GetRequiredService<JobDashboardOptions>();
            if (jobDashboardOptions.Enabled)
            {
                app.UseHangfireDashboard("/dashboard", new DashboardOptions
                {
#if DEBUG
#else
                    Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = false,
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        Users = new []
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = jobDashboardOptions.Username,
                                PasswordClear =  jobDashboardOptions.Password
                            } 
                        }
                    }) }
#endif
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