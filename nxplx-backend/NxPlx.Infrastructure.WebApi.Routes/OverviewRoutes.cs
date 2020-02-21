using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Dto.Models;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class OverviewRoutes
    {
        private const int MaxCacheAge = 15;
        public static void BindHandlers(IRouter router)
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
            
            res.AddHeader("Cache-Control", $"max-age={MaxCacheAge}");
            return await res.SendString(cached, "application/json");
        }

        private static async Task<HandlerType> GetOverview(Request req, Response res)
        {
            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>();

            var libs = session.User.LibraryAccessIds;
            if (session.IsAdmin) libs = await ctx.Libraries.ProjectMany(null, l => l.Id).ToListAsync();
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
                var genres = await ctx.Genres.Many().ToListAsync();
                
                var dtoMapper = container.Resolve<IDtoMapper>();
                return dtoMapper.Map<Genre, GenreDto>(genres);
            });
        }
        private static async Task<IEnumerable<OverviewElementDto>> BuildOverview(ResolveContainer container, IReadNxplxContext ctx, List<int> libs)
        {
            var seriesDetails = await ctx.EpisodeFiles
                .ProjectMany(ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId),
                    ef => ef.SeriesDetails, ef => ef.SeriesDetails)
                .ToListAsync();

            var filmDetails = await ctx.FilmFiles
                .ProjectMany(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId), 
                    ff => ff.FilmDetails, ff => ff.FilmDetails, ff => ff.FilmDetails.BelongsInCollection)
                .ToListAsync();
            
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

            return overview.OrderBy(oe => oe.title);
        }
    }

}