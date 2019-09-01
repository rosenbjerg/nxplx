using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Logging;
using NxPlx.Integrations.TMDBApi;
using NxPlx.Models;
using NxPlx.Models.File;
using NxPlx.Services.Caching;
using NxPlx.Services.Database;
using NxPlx.Services.Index;
using NxPlx.WebApi.Routers;
using Red;

namespace NxPlx.WebApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cfg = ConfigurationService.Current;

            {
                var registration = new RegistrationContainer();
                registration.Register<ICachingService, RedisCachingService>();
                registration.Register<ILogger, NLogger>();
                registration.Register<IDetailsMapper, TMDbMapper>();
                registration.Register<IDetailsApi, TmdbApi>();
                registration.Register<Indexer>();
            }
            
            var container = new ResolveContainer();
            var indexer = container.Resolve<Indexer>();
            
            await indexer.IndexMovieLibrary();
            await indexer.IndexSeriesLibrary();

           
            var server = new RedHttpServer(cfg.HttpPort);
            
            server.RespondWithExceptionDetails = true;
            
            var databaseContextManager = new DatabaseContextManager();

            
            
            server.ConfigureServices = services =>
            {
                databaseContextManager.Register(services);
            };
            databaseContextManager.Initialize();
            
            EpisodeRoutes.Register(server.CreateRouter("/series"));
            
            server.Get("/insert", async (req, res) =>
            {
                var dummyfile = new EpisodeFile
                {
                    FileSizeBytes = 12398,
                    Path = "/daw/daw/daw",
                    MediaDetails = new FFMpegProbeDetails(),
                    Added = DateTime.UtcNow,
                    LastWrite = DateTime.UtcNow
                };
                
                
                using (var context = new MediaContext())
                {
                    context.EpisodeFiles.Add(dummyfile);

                    await context.SaveChangesAsync();
                }
                
                return await res.SendString("ok");
            });
            
            server.Get("/fetch", async (req, res) =>
            {
                using (var context = new MediaContext())
                {
                    var files = await context.EpisodeFiles.Include(e => e.MediaDetails).ToListAsync();
                    return await res.SendJson(files);
                }
            });

            Console.WriteLine("NxPlx.Infrastructure.WebApi starting...");
            await server.RunAsync();
        }
        
        
    }
    
}