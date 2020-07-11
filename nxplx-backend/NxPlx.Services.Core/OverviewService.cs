using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class OverviewService
    {
        private readonly OperationContext _operationContext;
        private readonly DatabaseContext _databaseContext;
        private readonly IDistributedCache _distributedCache;
        private readonly IDtoMapper _dtoMapper;

        public OverviewService(OperationContext operationContext, DatabaseContext databaseContext, IDistributedCache distributedCache, IDtoMapper dtoMapper)
        {
            _operationContext = operationContext;
            _databaseContext = databaseContext;
            _distributedCache = distributedCache;
            _dtoMapper = dtoMapper;
        }
        
        public async Task<IEnumerable<OverviewElementDto>> GetOverview()
        {
            var libs = _operationContext.User.LibraryAccessIds;
            if (_operationContext.User.Admin) libs = await _databaseContext.Libraries.Select(l => l.Id).ToListAsync();
            var overviewCacheKey = "OVERVIEW:" + string.Join(',', libs.OrderBy(i => i));
            return await CachedResult(overviewCacheKey, () => BuildOverview(libs));
        }

        public Task<IEnumerable<GenreDto>> GetGenresOverview()
        {
            var overviewCacheKey = "OVERVIEW:GENRES";
            return CachedResult(overviewCacheKey, async () =>
            {
                var genres = await _databaseContext.Genre.AsNoTracking().ToListAsync();
                return _dtoMapper.Map<Genre, GenreDto>(genres);
            });
        }

        private async Task<T> CachedResult<T>(string key, Func<Task<T>> generator)
            where T : class
        {
            var cached = await _distributedCache.GetObjectAsync<T>(key);
            if (cached != null) return cached;
            
            var generated = await generator();
            await _distributedCache.SetObjectAsync(key, generated, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(3) });
            return generated;
        }
        
        private async Task<IEnumerable<OverviewElementDto>> BuildOverview(List<int> libs)
        {
            var seriesDetails = await _databaseContext.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId))
                .Select(ef => ef.SeriesDetails).Distinct()
                .ToListAsync();

            var filmDetails = await _databaseContext.FilmFiles
                .Where(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId))
                .Select(ff => ff.FilmDetails)
                .ToListAsync();
            
            var collections = filmDetails
                .Where(fd => fd.BelongsInCollectionId.HasValue)
                .GroupBy(fd => fd.BelongsInCollectionId)
                .Where(group => group.Count() > 1)
                .Select(group => group.First().BelongsInCollection)
                .ToList();

            var collectionIds = collections.Select(c => c.Id).ToList();
            var notInCollections = filmDetails.Where(fd => fd.BelongsInCollectionId == null || !collectionIds.Contains(fd.BelongsInCollectionId.Value)).ToList();
            
            var overview = new List<OverviewElementDto>(notInCollections.Count + seriesDetails.Count + collections.Count);
            overview.AddRange(_dtoMapper.Map<DbSeriesDetails, OverviewElementDto>(seriesDetails));
            overview.AddRange(_dtoMapper.Map<DbFilmDetails, OverviewElementDto>(notInCollections));
            overview.AddRange(_dtoMapper.Map<MovieCollection, OverviewElementDto>(collections));

            return overview.OrderBy(oe => oe.Title).ToList();
        }
    }
}