using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NxPlx.Application.Core.Settings;

namespace NxPlx.ApplicationHost.Api
{
    public abstract class ApplicationHostStartup
    {
        protected ApplicationHostStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            HostEnvironment = hostEnvironment;
            Configuration = configuration;
        }

        protected IHostEnvironment HostEnvironment { get; }
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
        
        protected void AddOptions<TSettings>(IServiceCollection services)
            where TSettings : class, ISettings, new()
        {
            services.AddOptions<TSettings>()
                .Bind(Configuration.GetSection(typeof(TSettings).Name.Replace("Settings", string.Empty)))
                .ValidateDataAnnotations();
            services.AddSingleton(typeof(TSettings), provider => provider.GetService<IOptions<TSettings>>().Value);
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServiceCollection(services);
            ConfigureDependencies(services);
        }
        
        public abstract void ConfigureServiceCollection(IServiceCollection services);
        
        public abstract void ConfigureDependencies(IServiceCollection services);
    }
}