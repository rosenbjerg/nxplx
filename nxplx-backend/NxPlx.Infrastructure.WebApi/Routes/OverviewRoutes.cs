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

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<MediaContext>();
            
            var libs = !session.IsAdmin ? session.LibraryAccess : ctx.Libraries.Select(l => l.Id).ToList();
            var overviewCacheKey = "OVERVIEW:" + string.Join(',', libs.OrderBy(i => i));

            var cacher = container.Resolve<ICachingService>();

            var cached = await cacher.GetAsync(overviewCacheKey);
            if (cached != null)
            {
                return await res.SendString(cached, "application/json");
            }
            
            var dtoMapper = container.Resolve<IDatabaseMapper>();

            var seriesDetails = await ctx.EpisodeFiles
                .Include(ef => ef.SeriesDetails)
                .Where(ef => ef.SeriesDetailsId != null && (session.IsAdmin || session.LibraryAccess.Contains(ef.PartOfLibraryId)))
                .Select(ef => ef.SeriesDetails)
                .Distinct()
                .ToListAsync();

            var filmDetails = await ctx.FilmFiles
                .Where(ff =>  ff.FilmDetailsId != null && (session.IsAdmin || session.LibraryAccess.Contains(ff.PartOfLibraryId)))
                .Select(ff => ff.FilmDetails)
                .Distinct()
                .ToListAsync();
            
            var overview = new List<OverviewElementDto>(filmDetails.Count + seriesDetails.Count);
            overview.AddRange(dtoMapper.MapMany<DbSeriesDetails, OverviewElementDto>(seriesDetails));
            overview.AddRange(dtoMapper.MapMany<DbFilmDetails, OverviewElementDto>(filmDetails));

            cached = System.Text.Json.JsonSerializer.Serialize(overview);
            await cacher.SetAsync(overviewCacheKey, cached, CacheKind.JsonResponse);

            return await res.SendString(cached, "application/json");
        }
    }

}