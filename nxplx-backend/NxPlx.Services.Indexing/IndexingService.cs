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
        private readonly DatabaseContext _context;
        private readonly ICacheClearer _cacheClearer;

        public IndexingService(
            IDetailsApi detailsApi,
            IDatabaseMapper databaseMapper,
            ILogger<IndexingService> logger,
            DatabaseContext context,
            ICacheClearer cacheClearer)
        {
            _detailsApi = detailsApi;
            _databaseMapper = databaseMapper;
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
            filmFile.Created = fileInfo.CreationTimeUtc;
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
                episodeFile.Created = fileInfo.CreationTimeUtc;
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
            var newDetails = await databaseDetails.GetUniqueNew(_context);
            newDetails.ForEach(film => film.Added = DateTime.UtcNow);
            await _context.AddRangeAsync(newDetails);
            await _context.SaveChangesAsync();
            await _cacheClearer.Clear("OVERVIEW");
            
            foreach (var detail in details)
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessFilmDetails(detail.Id));
            foreach (var movieCollection in movieCollections)
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessMovieCollection(movieCollection.Id));
            if (productionCompanies.Any())
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessProductionCompanies(productionCompanies.Select(n => n.Id).ToArray()));
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
            var details = await Da(newEpisodes, library);

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
            await _cacheClearer.Clear("OVERVIEW");
            
            foreach (var detail in details)
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessSeries(detail.Id));
            if (networks.Any())
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessNetworks(networks.Select(n => n.Id).ToArray()));
            if (productionCompanies.Any())
                BackgroundJob.Enqueue<ImageProcessor>(service => service.ProcessProductionCompanies(productionCompanies.Select(n => n.Id).ToArray()));
            foreach (var episodes in newEpisodes.GroupBy(e => e.SeriesDetailsId))
                BackgroundJob.Enqueue<IndexingService>(service => service.AnalyseEpisodeFiles(episodes.Select(e => e.Id).ToArray(), libraryId));
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

        private async Task<List<SeriesDetails>> Da(List<EpisodeFile> newEpisodes, Library library)
        {
            await FindSeriesDetails(newEpisodes);
            var details = new List<SeriesDetails>();
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