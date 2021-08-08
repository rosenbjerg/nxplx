using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NxPlx.Abstractions;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Events;
using NxPlx.Application.Mapping;
using NxPlx.Application.Services;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.ApplicationHost.Api.Middleware;
using NxPlx.Domain.Services.Commands;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Integrations.TMDb;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Services.Index;
using Serilog.Core;
using SessionOptions = NxPlx.Application.Core.Options.SessionOptions;

namespace NxPlx.ApplicationHost.Api
{
    public class ApiStartup : ApplicationHostStartup
    {
        public ApiStartup(IConfiguration configuration) : base(configuration) { }

        public ServiceProvider ConfigureOptions(IServiceCollection services)
        {
            AddOptions<ApiKeyOptions>(services);
            AddOptions<BuildOptions>(services);
            AddOptions<ConnectionStrings>(services);
            AddOptions<FolderOptions>(services);
            AddOptions<HostingOptions>(services);
            AddOptions<LoggingOptions>(services);
            AddOptions<JobDashboardOptions>(services);
            AddOptions<ApiDocumentationOptions>(services);
            AddOptions<SessionOptions>(services);
            return services.BuildServiceProvider();
        }
        
        public override void ConfigureServices(IServiceCollection services)
        {
            var optionsProvider = ConfigureOptions(services);
            
            services
                .AddMvc(options =>
                {
                    options.Filters.Add<Send404WhenNull>();
                })
                .AddJsonOptions(options => ConfigureJsonSerializer(options.JsonSerializerOptions));

            var connectionStrings = optionsProvider.GetRequiredService<ConnectionStrings>();
            var hostingOptions = optionsProvider.GetRequiredService<HostingOptions>();
            var apiDocumentationOptions = optionsProvider.GetRequiredService<ApiDocumentationOptions>();
            
            
            services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
            
            services.AddWebSockets(options => options.AllowedOrigins.Add(hostingOptions.Origin));
            services.AddApiDocumentation(apiDocumentationOptions);
            services.AddJobProcessing(connectionStrings);
            services.AddAutoMapper(typeof(DtoProfile), typeof(TMDbProfile));
            
            
            services.AddStackExchangeRedisCache(options => options.Configuration = connectionStrings.Redis);
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionStrings.Pgsql, b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));
            
            services.AddHttpContextAccessor();

            services.AddSingleton<ConnectionHub>();
            services.AddSingleton<IHttpSessionService, CookieSessionService>();
            services.AddSingleton<IRouteSessionTokenExtractor, RouteSessionTokenExtractor>();
            services.AddSingleton<ICacheClearer, RedisCacheClearer>();
            services.AddSingleton<IDetailsApi, TMDbApi>();

            services.AddScoped<ILogEventEnricher, CommonEventEnricher>();
            services.AddScoped<IIndexingService, IndexingService>();
            services.AddScoped<ConnectionAccepter, WebsocketConnectionAccepter>();
            services.AddScoped<IOperationContext>(serviceProvider => serviceProvider.GetRequiredService<OperationContext>());
            services.AddScoped<OperationContext>();
            services.AddScoped<ReadOnlyDatabaseContext>();
            
            services.AddScoped<TempFileService>();
            services.AddScoped<LibraryCleanupService>();
            services.AddScoped<LibraryMetadataService>();
            services.AddScoped<LibraryDeduplicationService>();
            services.AddScoped<FileAnalysisService>();

            services
                .AddEventHandlingFramework()
                .AddApplicationEventHandlers(typeof(Application.Services.AssemblyMarker))
                .AddDomainEventHandlers(typeof(Domain.Services.AssemblyMarker));
            
            services.Scan(scan => scan
                .FromAssemblyOf<CommandBase>()
                    .AddClasses(classes => classes.AssignableTo<CommandBase>())
                        .AsSelf()
                        .WithScopedLifetime()
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            InitializeDatabase(serviceProvider);

            app.UseMiddleware<OperationContextMiddleware>();
            app.UseMiddleware<LoggingEnrichingMiddleware>();
            app.UseMiddleware<ExceptionInterceptorMiddleware>();
            app.UseMiddleware<PerformanceInterceptorMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseForwardedHeaders();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            
            var jobDashboardOptions = serviceProvider.GetRequiredService<JobDashboardOptions>();
            app.UseJobDashboard("/api/dashboard", jobDashboardOptions);
            
            var apiDocumentationOptions = serviceProvider.GetRequiredService<ApiDocumentationOptions>();
            app.UseApiDocumentation("/api/swagger", apiDocumentationOptions);
            
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseStaticFileHandler("public");
        }

        private static void InitializeDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            databaseContext.Database.Migrate();
            
            var eventDispatcher = scope.ServiceProvider.GetRequiredService<IApplicationEventDispatcher>();
            var d = eventDispatcher.Dispatch(new CreateAdminCommand()).GetAwaiter().GetResult();
        }
    }
}