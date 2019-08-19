using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Configuration;
using NxPlx.Models;
using NxPlx.Models.File;
using NxPlx.Services.Caching;
using NxPlx.Services.Database;
using NxPlx.Services.Index;
using Red;

namespace NxPlx.WebApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var indexer = new Indexer();
//            await indexer.IndexFolder("\\\\raspberrypi.local\\data\\NxPlx test data");
            await indexer.IndexFolder("\\\\raspberrypi.local\\data\\Media");
            
            
            var cfg = ConfigurationService.Current;
            var server = new RedHttpServer(cfg.HttpPort);
            
            server.RespondWithExceptionDetails = true;
            
            var databaseContextManager = new DatabaseContextManager();
            
            server.Plugins.Register<ICachingService, RedisCachingService>(new RedisCachingService());
            
            server.ConfigureServices = services =>
            {
                databaseContextManager.Register(services);
            };
            databaseContextManager.Initialize();
            
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

            Console.WriteLine("NxPlx.WebApi starting...");
            await server.RunAsync();
        }
    }
}