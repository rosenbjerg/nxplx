using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Infrastructure.Database;
using NxPlx.Domain.Models;
using NxPlx.Domain.Models.Database;

namespace NxPlx.Services.Index
{
    public class LibraryDeduplicationService
    {
        private readonly DatabaseContext _databaseContext;
        
        public LibraryDeduplicationService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        public async Task DeduplicateFilmMetadata(DbFilmDetails[] details)
        {
            var newIds = details.Select(s => s.Id).ToList();
            var existingFilm = await _databaseContext.FilmDetails
                .Include(fd => fd.BelongsInCollection)
                .Where(s => newIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id);
            foreach (var detail in details)
            {
                if (existingFilm.TryGetValue(detail.Id, out var existing))
                {
                    existing.Popularity = detail.Popularity;
                    existing.VoteAverage = detail.VoteAverage;
                    existing.VoteCount = detail.VoteCount;
                    existing.Revenue = detail.Revenue;
                    existing.ImdbId = detail.ImdbId;
                    existing.BelongsInCollection = detail.BelongsInCollection;
                    existing.BelongsInCollectionId = detail.BelongsInCollectionId;
                }
                else
                {
                    existingFilm[detail.Id] = detail;
                    _databaseContext.Add(detail);
                }
            }
        }
        public async Task DeduplicateSeriesMetadata(DbSeriesDetails[] details)
        {
            var newIds = details.Select(s => s.Id).ToList();
            var existingSeries = await _databaseContext.SeriesDetails
                .Include(s => s.Seasons)
                .ThenInclude(s => s.Episodes)
                .Where(s => newIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id);
            foreach (var detail in details)
            {
                if (existingSeries.TryGetValue(detail.Id, out var existing))
                {
                    existing.Popularity = detail.Popularity;
                    existing.VoteAverage = detail.VoteAverage;
                    existing.VoteCount = detail.VoteCount;
                    existing.InProduction = detail.InProduction;
                    existing.LastAirDate = detail.LastAirDate;

                    MergeSeasonMetadata(detail, existing);
                }
                else
                {
                    existingSeries[detail.Id] = detail;
                    _databaseContext.Add(detail);
                }
            }
        }
        
        public async Task<int[]> DeduplicateMovieCollections(DbFilmDetails[] details)
        {
            var movieCollectionIds = details.Where(d => d.BelongsInCollection != null).Select(d => d.BelongsInCollection.Id).Distinct().ToList();
            var existing = await _databaseContext.MovieCollection.Where(mc => movieCollectionIds.Contains(mc.Id)).ToDictionaryAsync(mc => mc.Id);
            var newIds = new List<int>();
            foreach (var detail in details.Where(d => d.BelongsInCollection != null))
            {
                if (!existing.ContainsKey(detail.BelongsInCollection.Id))
                {
                    existing[detail.BelongsInCollection.Id] = detail.BelongsInCollection;
                    _databaseContext.Add(detail.BelongsInCollection);
                    newIds.Add(detail.BelongsInCollection.Id);
                }
                else
                {
                    detail.BelongsInCollection = existing[detail.BelongsInCollection.Id];
                }
            }

            return newIds.ToArray();
        }
        
        public Task<int[]> DeduplicateEntities<TDetails, TEntity>(TDetails[] details, Func<TDetails, List<TEntity>> entitySelector)
            where TEntity : EntityBase
        {
            return DeduplicateEntities(details, entitySelector, e => e.Id, ids => entity => ids.Contains(entity.Id));
        }
        
        public async Task<TKey[]> DeduplicateEntities<TDetails, TEntity, TKey>(
            TDetails[] details, 
            Func<TDetails, List<TEntity>> entitySelector, 
            Func<TEntity, TKey> idSelector, 
            Func<List<TKey>, Expression<Func<TEntity, bool>>> existingSelector)
            where TEntity : class
            where TKey : notnull
        {
            var ids = details.SelectMany(d => entitySelector(d).Select(idSelector)).Distinct().ToList();
            var existing = await _databaseContext.Set<TEntity>().Where(existingSelector(ids)).ToDictionaryAsync(idSelector);
            var newIds = new List<TKey>();
            foreach (var detail in details)
            {
                var attachedEntities = entitySelector(detail);
                foreach (var entity in attachedEntities.Where(entity => !existing.ContainsKey(idSelector(entity))))
                {
                    existing[idSelector(entity)] = entity;
                    _databaseContext.Add(entity);
                    newIds.Add(idSelector(entity));
                }
                
                var deduplicated = attachedEntities.Select(entity => existing[idSelector(entity)]).ToList();
                attachedEntities.Clear();
                attachedEntities.AddRange(deduplicated);
            }

            return newIds.ToArray();
        }
        
        private void MergeSeasonMetadata(DbSeriesDetails detail, DbSeriesDetails existing)
        {
            foreach (var season in detail.Seasons)
            {
                var existingSeason = existing.Seasons.FirstOrDefault(s => s.SeasonNumber == season.SeasonNumber);
                if (existingSeason != null)
                {
                    existingSeason.Overview = season.Overview;
                    var missingEpisodes = season.Episodes.Where(e => existingSeason.Episodes.All(ee => e.Id != ee.Id))
                        .ToList();
                    existingSeason.Episodes.AddRange(missingEpisodes);
                    _databaseContext.AddRange(missingEpisodes);
                }
                else
                {
                    existing.Seasons.Add(season);
                    _databaseContext.Add(season);
                }
            }
        }
    }
}