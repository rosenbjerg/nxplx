using System;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.ApplicationHost.Api.Logging;
using NxPlx.Core.Services;
using NxPlx.Core.Services.Commands;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Integrations.TMDb;
using NxPlx.Models;
using NxPlx.Services.Database;
using NxPlx.Services.Index;
using Serilog.Core;

namespace NxPlx.ApplicationHost.Api
{
    public class Startup : ApplicationHostStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }
        
        public override void ConfigureServiceCollection(IServiceCollection services)
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
            services.AddHangfireServer(options => options.Queues = new[] { "default", "indexing" });
            
            services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

            var hostingSettings = Configuration.GetSection("Hosting").Get<HostingOptions>();
            if (hostingSettings.Swagger) 
                services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "NxPlx API", Version = "v1" }));
            
            services.AddWebSockets(options => options.AllowedOrigins.Add(hostingSettings.Origin));
        }

        public override void ConfigureDependencies(IServiceCollection services)
        {
            var connectionStrings = Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();

            HangfireContext.EnsureCreated(connectionStrings.HangfirePgsql);
            ConfigureHangfire(GlobalConfiguration.Configuration);
            services.AddHangfire(ConfigureHangfire);
            services.AddStackExchangeRedisCache(options => options.Configuration = connectionStrings.Redis);
            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionStrings.Pgsql).UseLazyLoadingProxies());
            
            services.AddSingleton(typeof(ConnectionHub));
            services.AddSingleton(typeof(IHttpSessionService), typeof(CookieSessionService));
            services.AddSingleton(typeof(IDatabaseMapper), typeof(DatabaseMapper));
            services.AddSingleton(typeof(IDtoMapper), typeof(DtoMapper));
            services.AddSingleton(typeof(AdminCommandService));
            
            services.AddScoped<ILogEventEnricher, CommonEventEnricher>();
            services.AddScoped(typeof(IIndexer), typeof(IndexingService));
            services.AddScoped(typeof(IDetailsApi), typeof(TMDbApi));
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
            
            Register(typeof(CommandBase), services.AddScoped!);
        }

        private static void Register(Type type, Func<Type, IServiceCollection> register)
        {
            var types = type.Assembly.ExportedTypes.Where(t => !t.IsAbstract && type.IsAssignableFrom(t)).ToList();
            foreach (var t in types) register(t);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext)
        {
            var hostingOptions = Configuration.GetSection("Hosting").Get<HostingOptions>();
            InitializeDatabase(databaseContext);

            app.UseSpaStaticFiles();
            app.UseCors(builder =>
            {
                builder.WithOrigins(hostingOptions.Origin)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
            app.UseForwardedHeaders();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            if (hostingOptions.HangfireDashboard) app.UseHangfireDashboard("/dashboard");
            if (hostingOptions.Swagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NxPlx API"));
            }
            
            app.UseMiddleware<LoggingInterceptorMiddleware>();
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
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