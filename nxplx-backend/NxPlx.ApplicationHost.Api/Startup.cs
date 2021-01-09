using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Mapping;
using NxPlx.ApplicationHost.Api.Logging;
using NxPlx.Core.Services;
using NxPlx.Core.Services.Commands;
using NxPlx.Core.Services.EventHandlers;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Integrations.TMDb;
using NxPlx.Models;
using NxPlx.Infrastructure.Database;
using NxPlx.Services.Index;
using Serilog.Core;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.ApplicationHost.Api
{
    public class Startup : ApplicationHostStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }
        
        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(options => ConfigureJsonSerializer(options.JsonSerializerOptions));
            
            AddOptions<ApiKeyOptions>(services);
            AddOptions<ConnectionStrings>(services);
            AddOptions<FolderOptions>(services);
            AddOptions<HostingOptions>(services);
            AddOptions<LoggingOptions>(services);
            AddOptions<NxPlx.Application.Core.Options.SessionOptions>(services);

            services.AddSpaStaticFiles(options => options.RootPath = "public");
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Math.Max(Environment.ProcessorCount - 1, 2);
                options.Queues = JobQueueNames.All;
            });
            
            services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

            var hostingSettings = Configuration.GetSection("Hosting").Get<HostingOptions>();
            services.AddWebSockets(options => options.AllowedOrigins.Add(hostingSettings.Origin));
            if (hostingSettings.ApiDocumentation) 
                services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "NxPlx API", Version = "v1" }));
            
            
            var connectionStrings = Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            
            services.AddAutoMapper(typeof(MappingAssemblyMarker));
            HangfireContext.EnsureCreated(connectionStrings.HangfirePgsql);
            ConfigureHangfire(GlobalConfiguration.Configuration);
            services.AddHangfire(ConfigureHangfire);
            services.AddStackExchangeRedisCache(options => options.Configuration = connectionStrings.Redis);
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionStrings.Pgsql, b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));
            
            services.AddHttpContextAccessor();

            services.AddSingleton<ConnectionHub>();
            services.AddSingleton<IHttpSessionService, CookieSessionService>();
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext)
        {
            var hostingOptions = Configuration.GetSection("Hosting").Get<HostingOptions>();
            InitializeDatabase(databaseContext);

            app.UseMiddleware<LoggingEnrichingMiddleware>();
            app.UseMiddleware<ExceptionInterceptorMiddleware>();
            app.UseMiddleware<PerformanceInterceptorMiddleware>();
            app.UseForwardedHeaders();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            if (hostingOptions.HangfireDashboard) app.UseHangfireDashboard("/dashboard");
            if (hostingOptions.ApiDocumentation)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NxPlx API"));
            }
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.GetFullPath("public"))
            });
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.Use(FallbackMiddlewareHandler);
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
        
        private static readonly Regex HashRegex = new("\\.[0-9a-f]{5}\\.", RegexOptions.Compiled); 
        private static readonly FileExtensionContentTypeProvider FileExtensionContentTypeProvider = new(); 
        private static async Task FallbackMiddlewareHandler(HttpContext context, Func<Task> next)
        {
            var path = context.Request.Path.ToString().TrimStart('/');
            var file = Path.Combine("public", path);
            var fileInfo = File.Exists(file)
                ? new FileInfo(file)
                : new FileInfo(Path.Combine("public", "index.html"));
            if (!context.Response.HasStarted)
            {
                if (HashRegex.IsMatch(path))
                    context.Response.Headers.Add("Cache-Control", "max-age=2592000");
                
                if(!FileExtensionContentTypeProvider.TryGetContentType(fileInfo.Name, out var contentType))
                    contentType = "application/octet-stream";
                context.Response.ContentType = contentType;
                context.Response.StatusCode = 200;
            }
            await context.Response.SendFileAsync(new PhysicalFileInfo(fileInfo));
            await context.Response.CompleteAsync();
        }
    }
}