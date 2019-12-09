using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Database;
using NxPlx.Models.Dto.Models;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
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
            await using var ctx = container.Resolve<IReadMediaContext>();

            var libs = session.User.LibraryAccessIds;
            if (session.IsAdmin)
            {
                libs = await ctx.Libraries.ProjectMany(null, l => l.Id);
            }
            var overviewCacheKey = "OVERVIEW:" + string.Join(',', libs.OrderBy(i => i));

            var cacher = container.Resolve<ICachingService>();

            var cached = await cacher.GetAsync(overviewCacheKey);
            if (cached != null)
            {
                return await res.SendString(cached, "application/json");
            }
            
            var dtoMapper = container.Resolve<IDatabaseMapper>();

            var seriesDetails = await ctx.EpisodeFiles.ProjectMany(
                ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId), ef => ef.SeriesDetails, ef => ef.SeriesDetails);

            var filmDetails = await ctx.FilmFiles
                .ProjectMany(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId), ff => ff.FilmDetails, ff => ff.FilmDetails);
            
            var overview = new List<OverviewElementDto>(filmDetails.Count + seriesDetails.Count);
            overview.AddRange(dtoMapper.MapMany<DbSeriesDetails, OverviewElementDto>(seriesDetails));
            overview.AddRange(dtoMapper.MapMany<DbFilmDetails, OverviewElementDto>(filmDetails));

            cached = System.Text.Json.JsonSerializer.Serialize(overview);
            await cacher.SetAsync(overviewCacheKey, cached, CacheKind.JsonResponse);

            return await res.SendString(cached, "application/json");
        }
    }

}