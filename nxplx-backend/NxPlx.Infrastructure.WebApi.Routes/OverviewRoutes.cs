using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Film;
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
            await using var ctx = container.Resolve<IReadContext>();

            var libs = session.User.LibraryAccessIds;
            if (session.IsAdmin) libs = await ctx.Libraries.ProjectMany(null, l => l.Id);
            
            var overviewCacheKey = "OVERVIEW:" + string.Join(',', libs.OrderBy(i => i));
            var cacher = container.Resolve<ICachingService>();
            var cached = await cacher.GetAsync(overviewCacheKey);
            if (cached == null)
            {
                cached = await BuildOverview(container, ctx, libs);
                await cacher.SetAsync(overviewCacheKey, cached, CacheKind.JsonResponse);
            }
            
            return await res.SendString(cached, "application/json");
        }

        private static async Task<string> BuildOverview(ResolveContainer container, IReadContext ctx, List<int> libs)
        {

            var seriesDetails = await ctx.EpisodeFiles.ProjectMany(
                ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId), ef => ef.SeriesDetails,
                ef => ef.SeriesDetails);

            var filmDetails = await ctx.FilmFiles
                .ProjectMany(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId), ff => ff.FilmDetails,
                    ff => ff.FilmDetails);

            var collections = filmDetails
                .Where(fd => fd.BelongsInCollectionId.HasValue)
                .GroupBy(fd => fd.BelongsInCollection)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            var collectionIds = collections.Select(c => c.Id).ToList();
            var notInCollections = filmDetails.Where(fd => fd.BelongsInCollectionId == null || collectionIds.Contains(fd.BelongsInCollectionId.Value)).ToList();
            
            var dtoMapper = container.Resolve<IDtoMapper>();
            var overview = new List<OverviewElementDto>(notInCollections.Count + seriesDetails.Count + collections.Count);
            overview.AddRange(dtoMapper.Map<DbSeriesDetails, OverviewElementDto>(seriesDetails));
            overview.AddRange(dtoMapper.Map<DbFilmDetails, OverviewElementDto>(notInCollections));
            overview.AddRange(dtoMapper.Map<MovieCollection, OverviewElementDto>(collections));

            var cached = System.Text.Json.JsonSerializer.Serialize(overview);
            return cached;
        }
    }

}