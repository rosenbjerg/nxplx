using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Mapping;
using NxPlx.Application.Services;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.ApplicationHost.Api.Extensions;
using NxPlx.ApplicationHost.Api.Middleware;
using NxPlx.Domain.Services.Commands;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Database.Repositories;
using NxPlx.Infrastructure.Events;
using NxPlx.Integrations.ImageSharp;
using NxPlx.Integrations.TMDb;
using NxPlx.Services.Index;
using Serilog.Core;

namespace NxPlx.ApplicationHost.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = PrepareWebHostBuilder(args);
            
            var app = hostBuilder.Build();
            app.UseMiddleware<OperationContextMiddleware>();
            app.UseMiddleware<LoggingEnrichingMiddleware>();
            app.UseMiddleware<ExceptionInterceptorMiddleware>();
            app.UseMiddleware<PerformanceInterceptorMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();

            
            app.UseForwardedHeaders();
            app.UseApiDocumentation();
            
            
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapJobDashboard();
            });
            app.UseStaticFileHandler("public");

            if (args.Contains("--auto-migrate"))
            {
                await app.InitializeDatabase(CancellationToken.None);
            }

            await app.RunAsync();
        }

        public static WebApplicationBuilder PrepareWebHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host
                .ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.AddEnvironmentVariables("NxPlx_"))
                .UseNxplxSerilog("Api");
            
            builder.Services.AddNxPlxOptions<ApiKeyOptions>(builder.Configuration);
            builder.Services.AddNxPlxOptions<BuildOptions>(builder.Configuration);
            builder.Services.AddNxPlxOptions<ConnectionStrings>(builder.Configuration);
            builder.Services.AddNxPlxOptions<FolderOptions>(builder.Configuration);
            builder.Services.AddNxPlxOptions<HostingOptions>(builder.Configuration);
            builder.Services.AddNxPlxOptions<LoggingOptions>(builder.Configuration);
            builder.Services.AddNxPlxOptions<JobDashboardOptions>(builder.Configuration);
            builder.Services.AddNxPlxOptions<ApiDocumentationOptions>(builder.Configuration);
            var optionsProvider = builder.Services.BuildServiceProvider();

            builder.Services
               .AddControllers(options => { options.Filters.Add<Send404WhenNull>(); })
               .AddJsonOptions(options => options.JsonSerializerOptions.ApplyDefaultConfiguration());

            var connectionStrings = optionsProvider.GetRequiredService<ConnectionStrings>();
            var hostingOptions = optionsProvider.GetRequiredService<HostingOptions>();
            
            
            builder.Services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
            
            builder.Services.AddWebSockets(options => options.AllowedOrigins.Add(hostingOptions.Origin));
            builder.Services.AddApiDocumentation();
            builder.Services.AddJobProcessing(connectionStrings);
            builder.Services.AddImageSharpImageService();
            builder.Services.AddAutoMapper(typeof(DtoProfile), typeof(TMDbProfile));

            builder.Services.AddStackExchangeRedisCache(options => options.Configuration = connectionStrings.Redis);
            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionStrings.Pgsql, b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));
            builder.Services.AddDbContext<HangfireContext>(options => options.UseNpgsql(connectionStrings.HangfirePgsql));
            
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<ConnectionHub>();
            builder.Services.AddSingleton<IHttpSessionService, CookieSessionService>();
            builder.Services.AddSingleton<IRouteSessionTokenExtractor, RouteSessionTokenExtractor>();
            builder.Services.AddSingleton<IDetailsApi, TMDbApi>();

            builder.Services.AddScoped<ILogEventEnricher, CommonEventEnricher>();
            builder.Services.AddScoped<IIndexingService, IndexingService>();
            builder.Services.AddScoped<ConnectionAccepter, WebsocketConnectionAccepter>();
            builder.Services.AddScoped<IOperationContext>(serviceProvider => serviceProvider.GetRequiredService<OperationContext>());
            builder.Services.AddScoped<OperationContext>();
            builder.Services.AddScoped<ReadOnlyDatabaseContext>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            
            builder.Services.AddScoped<TempFileService>();
            builder.Services.AddScoped<LibraryCleanupService>();
            builder.Services.AddScoped<LibraryMetadataService>();
            builder.Services.AddScoped<LibraryDeduplicationService>();
            builder.Services.AddScoped<FileAnalysisService>();

            builder.Services
                .AddEventHandlingFramework()
                .AddApplicationEventHandlers(typeof(AssemblyMarker))
                .AddDomainEventHandlers(typeof(Domain.Services.AssemblyMarker));
            
            builder.Services.Scan(scan => scan
                .FromAssemblyOf<CommandBase>()
                    .AddClasses(classes => classes.AssignableTo<CommandBase>())
                        .AsSelf()
                        .WithScopedLifetime()
            );

            return builder;
        }
    }
}