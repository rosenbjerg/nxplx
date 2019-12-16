using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class IndexingRoutes
    {
        public static void Register(IRouter router)
        {
            router.Post("", Authenticated.Admin, IndexLibraries);
            router.Post("/all", Authenticated.Admin, IndexAllLibraries);
        }

        private static async Task<HandlerType> IndexLibraries(Request req, Response res)
        {
            var container = ResolveContainer.Default();

            var libIds = req.ParseBody<JsonValue<int[]>>().value;
            
            IEnumerable<Library> libraries;
            await using (var ctx = container.Resolve<IReadMediaContext>())
            {
                libraries = await ctx.Libraries.Many(l => libIds.Contains(l.Id));
            }

            await res.SendStatus(HttpStatusCode.OK);
            await IndexAndBroadcast(container, libIds, libraries);
            return HandlerType.Final;
        }

        private static async Task<HandlerType> IndexAllLibraries(Request req, Response res)
        {
            var container = ResolveContainer.Default();

            IEnumerable<Library> libraries;
            await using (var ctx = container.Resolve<IReadMediaContext>())
            {
                libraries = await ctx.Libraries.Many();
            }
            var libIds = libraries.Select(l => l.Id).ToArray();
            
            await res.SendStatus(HttpStatusCode.OK);
            await IndexAndBroadcast(container, libIds, libraries);
            return HandlerType.Final;
        }

        private static async Task IndexAndBroadcast(ResolveContainer container, int[] libIds, IEnumerable<Library> libraries)
        {
            var indexer = container.Resolve<IIndexer>();
            var broadcaster = container.Resolve<IBroadcaster>();
            await broadcaster.BroadcastAll(new Broadcast<int[]>("indexing:started", libIds));
            await indexer.IndexLibraries(libraries);
            await broadcaster.BroadcastAll(new Broadcast<int[]>("indexing:completed", libIds));
        }
    }
}