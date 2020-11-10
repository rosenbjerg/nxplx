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
using NxPlx.Infrastructure.Database;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Core.Services
{
    public class OverviewService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDistributedCache _distributedCache;
        private readonly IDtoMapper _dtoMapper;
        private readonly IMapper _mapper;
        private readonly UserContextService _userContextService;

        public OverviewService(DatabaseContext databaseContext, IDistributedCache distributedCache, IDtoMapper dtoMapper, IMapper mapper, UserContextService userContextService)
        {
            _databaseContext = databaseContext;
            _distributedCache = distributedCache;
            _dtoMapper = dtoMapper;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        
        public async Task<IEnumerable<OverviewElementDto>> GetOverview()
        {
            var currentUser = await _userContextService.GetUser();
            var libs = currentUser.LibraryAccessIds;
            if (currentUser.Admin) libs = await _databaseContext.Libraries.Select(l => l.Id).ToListAsync();
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
            var seriesDetailIds = await _databaseContext.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId))
                .Select(ef => ef.SeriesDetailsId).Distinct()
                .ToListAsync();

            var seriesDetails = await _databaseContext.SeriesDetails
                .Where(sd => seriesDetailIds.Contains(sd.Id))
                .Project<DbSeriesDetails, OverviewElementDto>(_dtoMapper)
                .ToListAsync();
            
            var filmDetails = await _databaseContext.FilmFiles
                .Where(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId))
                .Select(ff => ff.FilmDetails).Distinct()
                .ToListAsync();
            
            var collections = filmDetails
                .Where(fd => fd.BelongsInCollectionId.HasValue)
                .GroupBy(fd => fd.BelongsInCollectionId)
                .Where(group => group.Count() > 1)
                .Select(group => group.First().BelongsInCollection)
                .ToList();

            var collectionIds = collections.Select(c => c.Id).ToList();
            var notInCollections = filmDetails.Where(fd => fd.BelongsInCollectionId == null || !collectionIds.Contains(fd.BelongsInCollectionId.Value)).ToList();

            var overview = new List<OverviewElementDto>(seriesDetails.Count + notInCollections.Count + collections.Count);
            overview.AddRange(seriesDetails);
            overview.AddRange(_mapper.Map<List<DbFilmDetails>, IEnumerable<OverviewElementDto>>(notInCollections));
            overview.AddRange(_mapper.Map<List<MovieCollection>, IEnumerable<OverviewElementDto>>(collections));

            return overview.OrderBy(oe => oe.Title).ToList();
        }
    }
}