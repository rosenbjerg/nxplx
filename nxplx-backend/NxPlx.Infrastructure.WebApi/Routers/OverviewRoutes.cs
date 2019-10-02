using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Database;
using NxPlx.Models.Dto.Models;
using NxPlx.Services.Database;
using Red;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public static class OverviewRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/", Authenticated.User, GetOverview);
        }

        private static async Task<HandlerType> GetOverview(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var overviewCacheKey = "OVERVIEW:" + string.Join(',', session.LibraryAccess.OrderBy(i => i));

            var container = new ResolveContainer();
            var cacher = container.Resolve<ICachingService>();

            var cached = await cacher.GetAsync(overviewCacheKey);
            if (cached != null)
            {
                return await res.SendString(cached, "application/json");
            }
            
            var dtoMapper = container.Resolve<IDatabaseMapper>();
            await using var ctx = container.Resolve<MediaContext>();

            var seriesDetails = await ctx.EpisodeFiles
                .Include(ef => ef.SeriesDetails)
                .Where(ef => ef.SeriesDetailsId != null && session.LibraryAccess.Contains(ef.PartOfLibraryId))
                .Select(ef => ef.SeriesDetails)
                .Distinct()
                .ToListAsync();

            var filmDetails = await ctx.FilmFiles
                .Where(ff =>  ff.FilmDetailsId != null && session.LibraryAccess.Contains(ff.PartOfLibraryId))
                .Select(ff => ff.FilmDetails)
                .Distinct()
                .ToListAsync();
            
            var overview = new List<OverviewElementDto>(filmDetails.Count + seriesDetails.Count);
            overview.AddRange(dtoMapper.MapMany<DbSeriesDetails, OverviewElementDto>(seriesDetails));
            overview.AddRange(dtoMapper.MapMany<DbFilmDetails, OverviewElementDto>(filmDetails));

            cached = JsonConvert.SerializeObject(overview);
            await cacher.SetAsync(overviewCacheKey, cached, CacheKind.JsonResponse);

            return await res.SendString(cached, "application/json");
        }
    }

}