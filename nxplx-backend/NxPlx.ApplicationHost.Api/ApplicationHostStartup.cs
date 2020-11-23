using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NxPlx.Application.Core.Options;

namespace NxPlx.ApplicationHost.Api
{
    public abstract class ApplicationHostStartup
    {
        protected ApplicationHostStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        protected IConfiguration Configuration { get; set; }

        protected void ConfigureHangfire(IGlobalConfiguration hangfireConfiguration)
        {
            hangfireConfiguration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Configuration.GetConnectionString("HangfirePgsql"));
        }
        
        protected void ConfigureJsonSerializer(JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new JsonStringEnumConverter());
        }
        
        protected void AddOptions<TOptions>(IServiceCollection services)
            where TOptions : class, INxplxOptions, new()
        {
            services.AddOptions<TOptions>()
                .Bind(Configuration.GetSection(typeof(TOptions).Name.Replace("Options", string.Empty)))
                .ValidateDataAnnotations();
            services.AddSingleton(typeof(TOptions), provider => provider.GetRequiredService<IOptions<TOptions>>().Value);
        }

        public abstract void ConfigureServices(IServiceCollection services);
    }
}