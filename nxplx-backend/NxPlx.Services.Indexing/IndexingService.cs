using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.File;
using NxPlx.Infrastructure.Database;
using Z.EntityFramework.Plus;

namespace NxPlx.Services.Index
{
    public class IndexingService : IIndexer
    {
        private readonly IDetailsApi _detailsApi;
        private readonly ILogger<IndexingService> _logger;
        private readonly DatabaseContext _context;
        private readonly ICacheClearer _cacheClearer;

        public IndexingService(
            IDetailsApi detailsApi,
            ILogger<IndexingService> logger,
            DatabaseContext context,
            ICacheClearer cacheClearer)
        {
            _detailsApi = detailsApi;
            _logger = logger;
            _context = context;
            _cacheClearer = cacheClearer;
        }

        public async Task IndexLibraries(int[] libraryIds)
        {
            var libraries = await _context.Libraries.AsNoTracking()
                .Where(l => libraryIds.Contains(l.Id))
                .Select(l => new {l.Id, l.Kind})
                .ToListAsync();

            var previousJob = string.Empty;
            foreach (var library in libraries)
            {
                previousJob = library.Kind switch
                {
                    LibraryKind.Film => IndexMovieLibrary(library.Id, previousJob),
                    LibraryKind.Series => IndexSeriesLibrary(library.Id, previousJob),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private string IndexMovieLibrary(int libraryId, string previousJob = "")
        {
            var deleteJobId = !string.IsNullOrEmpty(previousJob)
                ? BackgroundJob.ContinueJobWith<IndexingService>(previousJob, service => service.RemoveDeletedMovies(libraryId))
                : BackgroundJob.Enqueue<IndexingService>(service => service.RemoveDeletedMovies(libraryId));
            return BackgroundJob.ContinueJobWith<IndexingService>(deleteJobId, service => service.IndexNewMovies(libraryId));
        }
        
        private string IndexSeriesLibrary(int libraryId, string previousJob = "")
        {
            var deleteJobId = !string.IsNullOrEmpty(previousJob)
                ? BackgroundJob.ContinueJobWith<IndexingService>(previousJob, service => service.RemoveDeletedEpisodes(libraryId))
                : BackgroundJob.Enqueue<IndexingService>(service => service.RemoveDeletedEpisodes(libraryId));
            return BackgroundJob.ContinueJobWith<IndexingService>(deleteJobId, service => service.IndexNewEpisodes(libraryId));
        }

        [Queue(JobQueueNames.FileAnalysis)]
        public async Task AnalyseFilmFile(int filmFileId, int libraryId)
        {
            var filmFile = await _context.FilmFiles.FindAsync(filmFileId);
            var fileInfo = new FileInfo(filmFile.Path);
            filmFile.LastWrite = fileInfo.LastWriteTimeUtc;
            filmFile.FileSizeBytes = fileInfo.Length;
            filmFile.PartOfLibraryId = libraryId;
            filmFile.MediaDetails = await AnalyseMedia(filmFile.Path);
            filmFile.Subtitles = FileIndexer.IndexSubtitles(filmFile.Path);
            await _context.SaveChangesAsync();
        }
        
        [Queue(JobQueueNames.FileAnalysis)]
        public async Task AnalyseEpisodeFiles(int[] episodeFileIds, int libraryId)
        {
            var episodeFiles = await _context.EpisodeFiles.Where(e => episodeFileIds.Contains(e.Id)).ToListAsync();
            foreach (var episodeFile in episodeFiles)
            {
                var fileInfo = new FileInfo(episodeFile.Path);
                episodeFile.LastWrite = fileInfo.LastWriteTimeUtc;
                episodeFile.FileSizeBytes = fileInfo.Length;
                episodeFile.PartOfLibraryId = libraryId;
                episodeFile.MediaDetails = await AnalyseMedia(episodeFile.Path);
                episodeFile.Subtitles = FileIndexer.IndexSubtitles(episodeFile.Path);
            }
            await _context.SaveChangesAsync();
        }
        
        [Queue(JobQueueNames.FileIndexing)]
        public async Task IndexNewMovies(int libraryId)
        {
            var library = await _context.Libraries.FindAsync(libraryId);
            var currentFilePaths = new HashSet<string>(await _context.FilmFiles.Where(f => f.PartOfLibraryId == libraryId).Select(f => f.Path).ToListAsync());
            var newFiles = FileIndexer.FindFiles(library.Path, "*", "mp4").Where(filePath => !currentFilePaths.Contains(filePath));
            var newFilm = FileIndexer.IndexFilmFiles(newFiles, libraryId).ToList();
            var details = await FindFilmDetails(newFilm, library)!;
            if (!details.Any()) return;
            
            await DeduplicateEntities(details, d => d.Genres);
            await DeduplicateEntities(details, d => d.ProductionCountries, pc => pc.Iso3166_1);
            await DeduplicateEntities(details, d => d.SpokenLanguages, sl => sl.Iso639_1);
            var newProductionCompanyIds = await DeduplicateEntities(details, d => d.ProductionCompanies);
            var newMovieCollectionIds = await DeduplicateMovieCollections(details);

            _context.FilmFiles.AddRange(newFilm);
            await _context.SaveChangesAsync();
            await _cacheClearer.Clear("OVERVIEW");
            
            foreach (var detail in details)
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessFilmDetails(detail.Id));
            foreach (var movieCollectionId in newMovieCollectionIds)
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessMovieCollection(movieCollectionId));
            if (newProductionCompanyIds.Any())
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessProductionCompanies(newProductionCompanyIds));
            foreach (var filmFile in newFilm)
                BackgroundJob.Enqueue<IndexingService>(service => service.AnalyseFilmFile(filmFile.Id, libraryId));
        }

        [Queue(JobQueueNames.FileIndexing)]
        public async Task IndexNewEpisodes(int libraryId)
        {
            var library = await _context.Libraries.FindAsync(libraryId);
            var currentEpisodePaths = new HashSet<string>(await _context.EpisodeFiles.Where(e => e.PartOfLibraryId == libraryId).Select(e => e.Path).ToListAsync());
            var newFiles = FileIndexer.FindFiles(library.Path, "*", "mp4").Where(filePath => !currentEpisodePaths.Contains(filePath));
            var newEpisodes = FileIndexer.IndexEpisodeFiles(newFiles, library).ToList();
            var details = await FindSeriesDetails(newEpisodes, library);
            if (!details.Any()) return;

            await DeduplicateEntities(details, d => d.Genres);
            await DeduplicateEntities(details, d => d.CreatedBy);
            var newProductionCompanyIds = await DeduplicateEntities(details, d => d.ProductionCompanies);
            var newNetworkIds = await DeduplicateEntities(details, d => d.Networks);

            await DeduplicateSeriesMetadata(details);

            _context.AddRange(newEpisodes);
            await _context.SaveChangesAsync();
            await _cacheClearer.Clear("OVERVIEW");
            
            foreach (var detail in details)
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessSeries(detail.Id));
            if (newNetworkIds.Any())
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessNetworks(newNetworkIds));
            if (newProductionCompanyIds.Any())
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessProductionCompanies(newProductionCompanyIds));
            foreach (var episodes in newEpisodes.GroupBy(e => e.SeriesDetailsId))
                BackgroundJob.Enqueue<IndexingService>(service => service.AnalyseEpisodeFiles(episodes.Select(e => e.Id).ToArray(), libraryId));
        }

        private async Task DeduplicateSeriesMetadata(List<DbSeriesDetails> details)
        {
            var newIds = details.Select(s => s.Id).ToList();
            var existingSeries = await _context.SeriesDetails
                .Include(s => s.Seasons)
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
                    _context.Add(detail);
                }
            }
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
                    _context.AddRange(missingEpisodes);
                }
                else
                {
                    existing.Seasons.Add(season);
                    _context.Add(season);
                }
            }
        }

        private async Task<List<int>> DeduplicateMovieCollections(List<DbFilmDetails> details)
        {
            var movieCollectionIds = details.Where(d => d.BelongsInCollectionId != null).Select(d => d.BelongsInCollectionId!.Value).Distinct().ToList();
            var existing = await _context.MovieCollection.Where(mc => movieCollectionIds.Contains(mc.Id)).ToDictionaryAsync(mc => mc.Id);
            foreach (var detail in details.Where(d => d.BelongsInCollection != null))
            {
                if (!existing.ContainsKey(detail.BelongsInCollectionId!.Value))
                {
                    existing[detail.BelongsInCollectionId.Value] = detail.BelongsInCollection;
                    _context.Add(detail.BelongsInCollection);
                }
                else
                {
                    detail.BelongsInCollection = existing[detail.BelongsInCollectionId!.Value];
                }
            }

            return movieCollectionIds.Where(id => !existing.ContainsKey(id)).ToList();
        }
        private Task<List<int>> DeduplicateEntities<TDetails, TEntity>(List<TDetails> details, Func<TDetails, List<TEntity>> entitySelector)
            where TEntity : EntityBase
        {
            return DeduplicateEntities(details, entitySelector, e => e.Id);
        }
        private async Task<List<TKey>> DeduplicateEntities<TDetails, TEntity, TKey>(List<TDetails> details, Func<TDetails, List<TEntity>> entitySelector, Func<TEntity, TKey> idSelector)
            where TEntity : class
        {
            var ids = details.SelectMany(d => entitySelector(d).Select(idSelector)).Distinct().ToList();
            var existing = await _context.Set<TEntity>().Where(entity => ids.Contains(idSelector(entity))).ToDictionaryAsync(idSelector);
            foreach (var detail in details)
            {
                var attachedEntities = entitySelector(detail);
                foreach (var entity in attachedEntities.Where(entity => !existing.ContainsKey(idSelector(entity))))
                {
                    existing[idSelector(entity)] = entity;
                    _context.Add(entity);
                }

                var deduplicated = attachedEntities.Select(entity => existing[idSelector(entity)]).ToList();
                attachedEntities.Clear();
                attachedEntities.AddRange(deduplicated);
            }

            return ids.Where(id => !existing.ContainsKey(id)).ToList();
        }


        [Queue(JobQueueNames.FileIndexing)]
        public async Task RemoveDeletedMovies(int libraryId)
        {
            var currentFilePaths = await _context.FilmFiles.Where(f => f.PartOfLibraryId == libraryId).Select(e => new { e.Id, e.Path }).ToListAsync();
            var deletedIds = currentFilePaths.Where(file => !File.Exists(file.Path)).Select(file => file.Id).ToList();
            if (deletedIds.Any())
            {
                await RemoveWatchingProgress(deletedIds);
                await RemoveSubtitlePreferences(deletedIds);
                var deleted = await _context.FilmFiles.Where(f => deletedIds.Contains(f.Id)).DeleteAsync();
                await _context.SaveChangesAsync();
                await _cacheClearer.Clear("OVERVIEW");
                _logger.LogInformation("Deleted {DeletedAmount} film from Library {LibaryId} because files were removed", deleted, libraryId);
            }
        }
        
        [Queue(JobQueueNames.FileIndexing)]
        public async Task RemoveDeletedEpisodes(int libraryId)
        {
            var currentFilePaths = await _context.EpisodeFiles.Where(f => f.PartOfLibraryId == libraryId).Select(e => new { e.Id, e.Path }).ToListAsync();
            var deletedIds = currentFilePaths.Where(file => !File.Exists(file.Path)).Select(file => file.Id).ToList();
            if (deletedIds.Any())
            {
                await RemoveWatchingProgress(deletedIds);
                await RemoveSubtitlePreferences(deletedIds);
                var deleted = await _context.EpisodeFiles.Where(f => deletedIds.Contains(f.Id)).DeleteAsync();
                await _context.SaveChangesAsync();
                await _cacheClearer.Clear("OVERVIEW");
                _logger.LogInformation("Deleted {DeletedAmount} film from Library {LibaryId} because files were removed", deleted, libraryId);
            }
        }

        private static async Task<MediaDetails> AnalyseMedia(string path)
        {
            var analysis = await FFMpegCore.FFProbe.AnalyseAsync(path);
            return new MediaDetails
            {
                Duration = (float)analysis.Duration.TotalSeconds,
                AudioBitrate = analysis.PrimaryAudioStream.BitRate,
                AudioCodec = analysis.PrimaryAudioStream.CodecName,
                AudioChannelLayout = analysis.PrimaryAudioStream.ChannelLayout,
                AudioStreamIndex = analysis.PrimaryAudioStream.Index,
                VideoBitrate = analysis.PrimaryVideoStream.BitRate,
                VideoCodec = analysis.PrimaryVideoStream.CodecName,
                VideoHeight = analysis.PrimaryVideoStream.Height,
                VideoWidth = analysis.PrimaryVideoStream.Width,
                VideoAspectRatio = $"{analysis.PrimaryVideoStream.DisplayAspectRatio.Width}x{analysis.PrimaryVideoStream.DisplayAspectRatio.Height}",
                VideoBitDepth = analysis.PrimaryVideoStream.BitsPerRawSample,
                VideoFrameRate = (float)analysis.PrimaryVideoStream.FrameRate
            };
        }
        private async Task RemoveWatchingProgress(List<int> fileIds)
        {
            var progress = await _context.WatchingProgresses.Where(wp => fileIds.Contains(wp.FileId)).ToListAsync();
            _context.WatchingProgresses.RemoveRange(progress);
        }
        private async Task RemoveSubtitlePreferences(List<int> fileIds)
        {
            var progress = await _context.SubtitlePreferences.Where(sp => fileIds.Contains(sp.FileId)).ToListAsync();
            _context.SubtitlePreferences.RemoveRange(progress);
        }

        private Task<DbFilmDetails> FetchFilmDetails(int id, string language) => _detailsApi.FetchMovieDetails(id, language);
        private async Task<List<DbFilmDetails>> FindFilmDetails(IEnumerable<FilmFile> newFilm, Library library)
        {
            var allDetails = new Dictionary<int, DbFilmDetails>();
            var functionCache = new FunctionCache<int, string, DbFilmDetails>(FetchFilmDetails);
            
            foreach (var filmFile in newFilm)
            {
                var searchResults = await _detailsApi.SearchMovies(filmFile.Title, filmFile.Year);
                if (searchResults == null || !searchResults.Any())
                    searchResults = await _detailsApi.SearchMovies(filmFile.Title, 0);

                if (searchResults == null || !searchResults.Any())
                    continue;

                var actual = new Fastenshtein.Levenshtein(filmFile.Title);
                var selectedResult = searchResults.OrderBy(sr => actual.DistanceFrom(sr.Title)).First();

                var details = await functionCache.Invoke(selectedResult.Id, library.Language);
                if (details == null) continue;

                filmFile.FilmDetailsId = details.Id;
                allDetails[details.Id] = details;
            }

            return allDetails.Values.ToList();
        }

        private async Task FindSeriesDetails(List<EpisodeFile> newEpisodes)
        {
            var functionCache = new FunctionCache<string, Models.Details.Search.SeriesResult[]>(_detailsApi.SearchTvShows);
            
            foreach (var episodeFile in newEpisodes)
            {
                var searchResults = await functionCache.Invoke(episodeFile.Name);
                if (searchResults == null || !searchResults.Any())
                {
                    episodeFile.SeriesDetailsId = null;
                    continue;
                }
                
                var actual = new Fastenshtein.Levenshtein(episodeFile.Name);
                var selectedResult = searchResults.OrderBy(sr => actual.DistanceFrom(sr.Name)).First();
                
                episodeFile.SeriesDetailsId = selectedResult.Id;
            }
        }

        private async Task<List<DbSeriesDetails>> FindSeriesDetails(List<EpisodeFile> newEpisodes, Library library)
        {
            await FindSeriesDetails(newEpisodes);
            var details = new List<DbSeriesDetails>();
            var newSeries = newEpisodes.ToLookup(e => e.SeriesDetailsId);
            
            foreach (var series in newSeries)
            {
                if (series.Key == null) continue;
                var seasons = series.Select(e => e.SeasonNumber).Distinct().ToArray();
                var seriesDetails = await _detailsApi.FetchTvDetails(series.Key.Value, library.Language, seasons);
                details.Add(seriesDetails);
            }

            return details;
        }
    }
}