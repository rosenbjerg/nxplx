using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using Nxplx.Integrations.FFMpeg;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using Z.EntityFramework.Plus;

namespace NxPlx.Services.Index
{
    public class IndexingService : IIndexer
    {
        private readonly IDetailsApi _detailsApi;
        private readonly IDatabaseMapper _databaseMapper;
        private readonly ILogger<IndexingService> _logger;
        private readonly IDistributedCache _cachingService;
        private readonly DatabaseContext _context;

        public IndexingService(
            IDetailsApi detailsApi,
            IDatabaseMapper databaseMapper,
            ILogger<IndexingService> logger,
            IDistributedCache cachingService,
            DatabaseContext context)
        {
            _detailsApi = detailsApi;
            _databaseMapper = databaseMapper;
            _logger = logger;
            _cachingService = cachingService;
            _context = context;
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
                switch (library.Kind)
                {
                    case LibraryKind.Film:
                        previousJob = IndexMovieLibrary(library.Id, previousJob);
                        break;
                    case LibraryKind.Series:
                        previousJob = IndexSeriesLibrary(library.Id, previousJob);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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

        public async Task AnalyseFilmFile(int filmFileId, int libraryId)
        {
            var filmFile = await _context.FilmFiles.FindAsync(filmFileId);
            var fileInfo = new FileInfo(filmFile.Path);
            filmFile.Created = fileInfo.CreationTimeUtc;
            filmFile.LastWrite = fileInfo.LastWriteTimeUtc;
            filmFile.FileSizeBytes = fileInfo.Length;
            filmFile.PartOfLibraryId = libraryId;
            filmFile.MediaDetails = FFProbe.Analyse(filmFile.Path);
            filmFile.Subtitles = FileIndexer.IndexSubtitles(filmFile.Path);
            await _context.SaveChangesAsync();
        }
        
        public async Task AnalyseEpisodeFiles(int[] episodeFileIds, int libraryId)
        {
            var episodeFiles = await _context.EpisodeFiles.Where(e => episodeFileIds.Contains(e.Id)).ToListAsync();
            foreach (var episodeFile in episodeFiles)
            {
                var fileInfo = new FileInfo(episodeFile.Path);
                episodeFile.Created = fileInfo.CreationTimeUtc;
                episodeFile.LastWrite = fileInfo.LastWriteTimeUtc;
                episodeFile.FileSizeBytes = fileInfo.Length;
                episodeFile.PartOfLibraryId = libraryId;
                episodeFile.MediaDetails = FFProbe.Analyse(episodeFile.Path);
                episodeFile.Subtitles = FileIndexer.IndexSubtitles(episodeFile.Path);
            }
            await _context.SaveChangesAsync();
        }
        
        public async Task IndexNewMovies(int libraryId)
        {
            var library = await _context.Libraries.FindAsync(libraryId);
            var currentFilePaths = new HashSet<string>(await _context.FilmFiles.Where(f => f.PartOfLibraryId == libraryId).Select(f => f.Path).ToListAsync());
            var newFiles = FileIndexer.FindFiles(library.Path, "*", "mp4").Where(filePath => !currentFilePaths.Contains(filePath));
            var newFilm = FileIndexer.IndexFilmFiles(newFiles, libraryId).ToList();

            var details = await FindFilmDetails(newFilm, library);
            if (!details.Any()) return;
            
            var genres = await details.Where(d => d.Genres != null).SelectMany(d => d.Genres).GetUniqueNew(_context);
            var productionCountries = await details.Where(d => d.ProductionCountries != null).SelectMany(d => d.ProductionCountries).GetUniqueNew(pc => pc.Iso3166_1, _context);
            var spokenLanguages = await details.Where(d => d.SpokenLanguages != null).SelectMany(d => d.SpokenLanguages).GetUniqueNew(sl => sl.Iso639_1, _context);
            var productionCompanies = await details.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies).GetUniqueNew(_context);
            var movieCollections = await details.Where(d => d.BelongsToCollection != null).Select(d => d.BelongsToCollection).GetUniqueNew(_context);
            
            _context.AddRange(genres);
            _context.AddRange(productionCountries);
            _context.AddRange(spokenLanguages);
            _context.AddRange(productionCompanies);
            _context.AddRange(movieCollections);
            _context.FilmFiles.AddRange(newFilm);
            var databaseDetails = _databaseMapper.Map<FilmDetails, DbFilmDetails>(details).ToList();
            databaseDetails.ForEach(film => film.Added = DateTime.UtcNow);
            var newDetails = await databaseDetails.GetUniqueNew(_context);
            await _context.AddRangeAsync(newDetails);
            await _context.SaveChangesAsync();
            
            var imageDownloads = IndexingHelperFunctions.AccumulateImageDownloads(details, productionCompanies, movieCollections);
            await IndexingHelperFunctions.DownloadImages(_detailsApi, imageDownloads);
            
            foreach (var filmFile in newFilm)
            {
                BackgroundJob.Enqueue<IndexingService>(service => service.AnalyseFilmFile(filmFile.Id, libraryId));
            }
        }
        public async Task IndexNewEpisodes(int libraryId)
        {
            var library = await _context.Libraries.FindAsync(libraryId);
            var currentEpisodePaths = new HashSet<string>(await _context.EpisodeFiles.Where(e => e.PartOfLibraryId == libraryId).Select(e => e.Path).ToListAsync());
            var newFiles = FileIndexer.FindFiles(library.Path, "*", "mp4").Where(filePath => !currentEpisodePaths.Contains(filePath));
            var newEpisodes = FileIndexer.IndexEpisodeFiles(newFiles, library).ToList();
            var details = await FindSeriesDetails(newEpisodes, library);

            var genres = await details.Where(d => d.Genres != null).SelectMany(d => d.Genres).GetUniqueNew(_context);
            var networks = await details.Where(d => d.Networks != null).SelectMany(d => d.Networks).GetUniqueNew(_context);
            var creators = await details.Where(d => d.CreatedBy != null).SelectMany(d => d.CreatedBy).GetUniqueNew(_context);
            var productionCompanies = await details.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies).GetUniqueNew(_context);
            
            _context.AddRange(genres);
            _context.AddRange(networks);
            _context.AddRange(creators);
            _context.AddRange(productionCompanies);
            _context.EpisodeFiles.AddRange(newEpisodes);
            var databaseDetails = _databaseMapper.Map<SeriesDetails, DbSeriesDetails>(details).ToList();
            databaseDetails.ForEach(series => series.Added = DateTime.UtcNow);
            await _context.AddOrUpdate(databaseDetails);
            
            await _context.SaveChangesAsync();
            var imageDownloads = IndexingHelperFunctions.AccumulateImageDownloads(details, networks, productionCompanies);
            await IndexingHelperFunctions.DownloadImages(_detailsApi, imageDownloads);
            
            foreach (var episodes in newEpisodes.GroupBy(e => e.SeriesDetailsId))
            {
                var ids = episodes.Select(e => e.Id).ToArray();
                BackgroundJob.Enqueue<IndexingService>(service => service.AnalyseEpisodeFiles(ids, libraryId));
            }
        }
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
                _logger.LogInformation("Deleted {DeletedAmount} film from Library {LibaryId} because files were removed", deleted, libraryId);
            }
        }
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
                _logger.LogInformation("Deleted {DeletedAmount} film from Library {LibaryId} because files were removed", deleted, libraryId);
            }
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

        private Task<FilmDetails> FetchFilmDetails(int id, string language) => _detailsApi.FetchMovieDetails(id, language);
        private async Task<List<FilmDetails>> FindFilmDetails(IEnumerable<FilmFile> newFilm, Library library)
        {
            var allDetails = new Dictionary<int, FilmDetails>();
            var functionCache = new FunctionCache<int, string, FilmDetails>(FetchFilmDetails);
            
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

        private async Task<List<SeriesDetails>> FindSeriesDetails(List<EpisodeFile> newEpisodes, Library library)
        {
            var allDetails = new HashSet<SeriesDetails>();
            var functionCache = new FunctionCache<int, string, SeriesDetails>(FetchSeriesDetails);
            
            foreach (var episodeFile in newEpisodes)
            {
                var searchResults = await _detailsApi.SearchTvShows(episodeFile.Name);
                if (searchResults == null || !searchResults.Any())
                {
                    episodeFile.SeriesDetailsId = null;
                    continue;
                }
                
                var actual = new Fastenshtein.Levenshtein(episodeFile.Name);
                var selectedResult = searchResults.OrderBy(sr => actual.DistanceFrom(sr.Name)).First();
                
                var seriesDetails = await functionCache.Invoke(selectedResult.Id, library.Language);
                episodeFile.SeriesDetailsId = seriesDetails.Id;
                allDetails.Add(seriesDetails);
            }

            return allDetails.ToList();
        }
        private Task<SeriesDetails> FetchSeriesDetails(int id, string language) => _detailsApi.FetchTvDetails(id, language);

    }
}