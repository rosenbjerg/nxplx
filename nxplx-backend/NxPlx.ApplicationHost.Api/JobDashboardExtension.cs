using System.Reflection;
using Hangfire;
using Hangfire.PostgreSql;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api
{
    public static class JobDashboardExtension
    {
        public static IServiceCollection AddJobProcessing(this IServiceCollection services, ConnectionStrings connectionStrings)
        {
            if (IsNSwagBuild())
                return services;

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(connectionStrings.HangfirePgsql));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Math.Max(Environment.ProcessorCount - 1, 2);
                options.Queues = JobQueueNames.All;
            });
            return services;
        }

        public static IEndpointRouteBuilder MapJobDashboard(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            if (IsNSwagBuild())
                return endpointRouteBuilder;

            endpointRouteBuilder.MapHangfireDashboard("/api/dashboard", new DashboardOptions
            {
                Authorization = new[] { new IntegratedHangfireAuthentication() }
            });
            return endpointRouteBuilder;
        }

        private static bool IsNSwagBuild()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "NSwag";
        }
    }
}