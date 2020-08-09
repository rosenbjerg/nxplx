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
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Mapping;
using NxPlx.ApplicationHost.Api.Logging;
using NxPlx.Core.Services;
using NxPlx.Core.Services.Commands;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Integrations.TMDb;
using NxPlx.Models;
using NxPlx.Services.Database;
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

            services.AddAutoMapper(typeof(AssemblyMarker));
            HangfireContext.EnsureCreated(connectionStrings.HangfirePgsql);
            ConfigureHangfire(GlobalConfiguration.Configuration);
            services.AddHangfire(ConfigureHangfire);
            services.AddStackExchangeRedisCache(options => options.Configuration = connectionStrings.Redis);
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionStrings.Pgsql, b => b.MigrationsAssembly("NxPlx.Infrastructure.Database"))
                    .UseLazyLoadingProxies());
            
            services.AddSingleton(typeof(ConnectionHub));
            services.AddSingleton(typeof(IHttpSessionService), typeof(CookieSessionService));
            services.AddSingleton(typeof(IDatabaseMapper), typeof(DatabaseMapper));
            services.AddSingleton(typeof(ICacheClearer), typeof(RedisCacheClearer));
            services.AddSingleton(typeof(IDtoMapper), typeof(DtoMapper));
            services.AddSingleton(typeof(IDetailsApi), typeof(TMDbApi));
            services.AddSingleton(typeof(AdminCommandService));
            
            services.AddScoped(typeof(ILogEventEnricher), typeof(CommonEventEnricher));
            services.AddScoped(typeof(IIndexer), typeof(IndexingService));
            services.AddScoped(typeof(TempFileService));
            services.AddScoped(typeof(ImageCreationService));
            services.AddScoped(typeof(ConnectionAccepter), typeof(WebsocketConnectionAccepter));
            services.AddScoped(typeof(OperationContext), _ => new OperationContext());
            services.AddScoped(typeof(AuthenticationService));
            services.AddScoped(typeof(EpisodeService));
            services.AddScoped(typeof(FilmService));
            services.AddScoped(typeof(LibraryService));
            services.AddScoped(typeof(NextEpisodeService));
            services.AddScoped(typeof(ProgressService));
            services.AddScoped(typeof(SessionService));
            services.AddScoped(typeof(SubtitleService));
            services.AddScoped(typeof(OverviewService));
            services.AddScoped(typeof(UserService));
            services.AddScoped(typeof(EditDetailsService));
            
            Register(typeof(CommandBase), services.AddScoped!);
        }

        private static void Register(Type type, Func<Type, IServiceCollection> register)
        {
            var types = type.Assembly.ExportedTypes.Where(t => !t.IsAbstract && type.IsAssignableFrom(t)).ToList();
            foreach (var t in types) register(t);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext, IMapper mapper)
        {
#if DEBUG
            // mapper.ConfigurationProvider.AssertConfigurationIsValid();
#endif
            
            var hostingOptions = Configuration.GetSection("Hosting").Get<HostingOptions>();
            InitializeDatabase(databaseContext);

            app.UseMiddleware<ExceptionInterceptorMiddleware>();
            app.UseForwardedHeaders();
            app.UseMiddleware<LoggingInterceptorMiddleware>();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            if (hostingOptions.HangfireDashboard) app.UseHangfireDashboard("/dashboard");
            if (hostingOptions.ApiDocumentation)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NxPlx API"));
            }
            
            app.UseStaticFiles();
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
        
        private static Regex HashRegex = new Regex("\\.[0-9a-f]{5}\\.", RegexOptions.Compiled); 
        private static async Task FallbackMiddlewareHandler(HttpContext context, Func<Task> next)
        {
            var path = context.Request.Path.ToString().TrimStart('/');
            var extension = Path.GetExtension(path);
            var file = Path.Combine("public", path);
            var fileInfo = File.Exists(file)
                ? new FileInfo(file)
                : new FileInfo(Path.Combine("public", "index.html"));
            if (!context.Response.HasStarted)
            {
                if (HashRegex.IsMatch(path))
                    context.Response.Headers.Add("Cache-Control", "max-age=2592000");
                
                var provider = new FileExtensionContentTypeProvider();
                if(!provider.TryGetContentType(fileInfo.Name, out var contentType))
                    contentType = "application/octet-stream";
                context.Response.ContentType = contentType;
                context.Response.StatusCode = 200;
            }
            await context.Response.SendFileAsync(new PhysicalFileInfo(fileInfo));
            await context.Response.CompleteAsync();
        }
    }
}