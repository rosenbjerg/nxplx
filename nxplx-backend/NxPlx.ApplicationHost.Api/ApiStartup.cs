using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Mapping;
using NxPlx.ApplicationHost.Api.Middleware;
using NxPlx.Core.Services;
using NxPlx.Core.Services.Commands;
using NxPlx.Core.Services.EventHandlers;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Integrations.TMDb;
using NxPlx.Models;
using NxPlx.Infrastructure.Database;
using NxPlx.Services.Index;
using Serilog.Core;

namespace NxPlx.ApplicationHost.Api
{
    public class ApiStartup : ApplicationHostStartup
    {
        public ApiStartup(IConfiguration configuration) : base(configuration) { }

        public ServiceProvider ConfigureOptions(IServiceCollection services)
        {
            AddOptions<ApiKeyOptions>(services);
            AddOptions<ConnectionStrings>(services);
            AddOptions<FolderOptions>(services);
            AddOptions<HostingOptions>(services);
            AddOptions<LoggingOptions>(services);
            AddOptions<JobDashboardOptions>(services);
            AddOptions<ApiDocumentationOptions>(services);
            AddOptions<NxPlx.Application.Core.Options.SessionOptions>(services);
            return services.BuildServiceProvider();
        }
        
        public override void ConfigureServices(IServiceCollection services)
        {
            var optionsProvider = ConfigureOptions(services);
            
            services
                .AddMvc()
                .AddJsonOptions(options => ConfigureJsonSerializer(options.JsonSerializerOptions));

            services.AddJobProcessing(optionsProvider);
            
            services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

            
            var hostingOptions = optionsProvider.GetRequiredService<HostingOptions>();
            services.AddWebSockets(options => options.AllowedOrigins.Add(hostingOptions.Origin));

            services.AddApiDocumentation(optionsProvider);
            
            services.AddAutoMapper(typeof(MappingAssemblyMarker));
            
            var connectionStrings = optionsProvider.GetRequiredService<ConnectionStrings>();
            
            services.AddStackExchangeRedisCache(options => options.Configuration = connectionStrings.Redis);
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionStrings.Pgsql, b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));
            
            services.AddHttpContextAccessor();

            services.AddSingleton<ConnectionHub>();
            services.AddSingleton<IHttpSessionService, CookieSessionService>();
            services.AddSingleton<IRouteSessionTokenExtractor, RouteSessionTokenExtractor>();
            services.AddSingleton<IDatabaseMapper, DatabaseMapper>();
            services.AddSingleton<ICacheClearer, RedisCacheClearer>();
            services.AddSingleton<IDtoMapper, DtoMapper>();
            services.AddSingleton<IDetailsApi, TMDbApi>();

            services.AddScoped<ILogEventEnricher, CommonEventEnricher>();
            services.AddScoped<IIndexer, IndexingService>();
            services.AddScoped<ConnectionAccepter, WebsocketConnectionAccepter>();
            services.AddScoped<IOperationContext>(serviceProvider => serviceProvider.GetRequiredService<OperationContext>());
            services.AddScoped<OperationContext>();
            
            services.AddScoped<TempFileService>();
            services.AddScoped<LibraryCleanupService>();
            services.AddScoped<LibraryMetadataService>();
            services.AddScoped<LibraryDeduplicationService>();
            services.AddScoped<FileAnalysisService>();

            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.Scan(scan => scan
                .FromAssemblyOf<IEventHandler>()
                    .AddClasses(classes => classes.AssignableTo<IEventHandler>())
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
                    .AddClasses(classes => classes.AssignableTo<CommandBase>())
                        .AsSelf()
                        .WithScopedLifetime()
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext, IServiceProvider serviceProvider)
        {
            InitializeDatabase(databaseContext);

            app.UseMiddleware<OperationContextMiddleware>();
            app.UseMiddleware<LoggingEnrichingMiddleware>();
            app.UseMiddleware<ExceptionInterceptorMiddleware>();
            app.UseMiddleware<PerformanceInterceptorMiddleware>();
            app.UseForwardedHeaders();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            
            app.UseJobDashboard("/dashboard", serviceProvider);
            app.UseApiDocumentation("/swagger", serviceProvider);
            
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseStaticFileHandler("public");
        }

        private static void InitializeDatabase(DatabaseContext databaseContext)
        {
            databaseContext.Database.Migrate();
            if (!databaseContext.Users.Any(u => u.Username == "admin"))
            {
                databaseContext.Add(new User
                {
                    Username = "admin",
                    PasswordHash = PasswordUtils.Hash("changemebaby"),
                    Admin = true
                });
                databaseContext.SaveChanges();
            }
        }
    }
}