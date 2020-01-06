using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
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
            router.Get("/genres", Authenticated.User, GetGenresOverview);
        }

        private static async Task<HandlerType> SendCachedJson(this Response res, string key, Func<Task<object>> builder)
        {
            var cache = ResolveContainer.Default.Resolve<ICachingService>();
            var cached = await cache.GetAsync(key);
            if (cached == null)
            {
                cached = System.Text.Json.JsonSerializer.Serialize(await builder());
                await cache.SetAsync(key, cached, CacheKind.JsonResponse);
            }
            
            return await res.SendString(cached, "application/json");
        }

        private static async Task<HandlerType> GetOverview(Request req, Response res)
        {
            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>();

            var libs = session.User.LibraryAccessIds;
            if (session.IsAdmin) libs = await ctx.Libraries.ProjectMany(null, l => l.Id);
            var overviewCacheKey = "OVERVIEW:" + string.Join(',', libs.OrderBy(i => i));
            return await res.SendCachedJson(overviewCacheKey, async () => await BuildOverview(container, ctx, libs));
        }

        private static async Task<HandlerType> GetGenresOverview(Request req, Response res)
        {
            var container = ResolveContainer.Default;
            var overviewCacheKey = "OVERVIEW:GENRES";
            return await res.SendCachedJson(overviewCacheKey, async () =>
            {
                await using var ctx = container.Resolve<IReadNxplxContext>();
                var genres = await ctx.Genres.Many();
                
                var dtoMapper = container.Resolve<IDtoMapper>();
                return dtoMapper.Map<Genre, GenreDto>(genres);
            });
        }
        private static async Task<List<OverviewElementDto>> BuildOverview(ResolveContainer container, IReadNxplxContext ctx, List<int> libs)
        {

            var seriesDetails = await ctx.EpisodeFiles.ProjectMany(
                ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId), ef => ef.SeriesDetails,
                ef => ef.SeriesDetails);

            var filmDetails = await ctx.FilmFiles
                .ProjectMany(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId), ff => ff.FilmDetails,
                    ff => ff.FilmDetails, ff => ff.FilmDetails.BelongsInCollection);
            
            var collections = filmDetails
                .Where(fd => fd.BelongsInCollectionId.HasValue)
                .GroupBy(fd => fd.BelongsInCollectionId)
                .Where(group => group.Count() > 1)
                .Select(group => group.First().BelongsInCollection)
                .ToList();

            var collectionIds = collections.Select(c => c.Id).ToList();
            var notInCollections = filmDetails.Where(fd => fd.BelongsInCollectionId == null || !collectionIds.Contains(fd.BelongsInCollectionId.Value)).ToList();
            
            var dtoMapper = container.Resolve<IDtoMapper>();
            var overview = new List<OverviewElementDto>(notInCollections.Count + seriesDetails.Count + collections.Count);
            overview.AddRange(dtoMapper.Map<DbSeriesDetails, OverviewElementDto>(seriesDetails));
            overview.AddRange(dtoMapper.Map<DbFilmDetails, OverviewElementDto>(notInCollections));
            overview.AddRange(dtoMapper.Map<MovieCollection, OverviewElementDto>(collections));

            return overview;
        }
    }

}