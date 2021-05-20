using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Services.Index
{
    public class IndexingService : IIndexingService
    {
        private readonly DatabaseContext _context;
        private readonly ICacheClearer _cacheClearer;
        private readonly LibraryDeduplicationService _deduplicationService;
        private readonly LibraryMetadataService _metadataService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public IndexingService(
            DatabaseContext context,
            ICacheClearer cacheClearer,
            LibraryDeduplicationService deduplicationService,
            LibraryMetadataService metadataService,
            IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _cacheClearer = cacheClearer;
            _deduplicationService = deduplicationService;
            _metadataService = metadataService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task IndexLibraries(int[] libraryIds)
        {
            var libraries = await _context.Libraries.AsNoTracking()
                .Where(l => libraryIds.Contains(l.Id))
                .Select(l => new {l.Id, l.Kind})
                .ToListAsync();

            if (libraries.Any(l => l.Kind == LibraryKind.Film))
                _backgroundJobClient.Enqueue<IndexingService>(service => service.IndexGenres(LibraryKind.Film, "en-UK"));
            if (libraries.Any(l => l.Kind == LibraryKind.Series))
                _backgroundJobClient.Enqueue<IndexingService>(service => service.IndexGenres(LibraryKind.Series, "en-UK"));
            
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

        [Queue(JobQueueNames.FileIndexing)]
        public async Task IndexNewMovies(int libraryId)
        {
            var library = await _context.Libraries.SingleAsync(l => l.Id == libraryId);
            var currentFilePaths = new HashSet<string>(await _context.FilmFiles.Where(f => f.PartOfLibraryId == libraryId).Select(f => f.Path).ToListAsync());
            var newFiles = FileIndexer.FindFiles(library.Path, "*", "mp4").Where(filePath => !currentFilePaths.Contains(filePath));
            var newFilm = FileIndexer.IndexFilmFiles(newFiles, libraryId).ToList();
            var details = await _metadataService.FindFilmDetails(newFilm, library)!;
            if (!details.Any()) return;
            
            await _deduplicationService.DeduplicateEntities(details, d => d.Genres);
            await _deduplicationService.DeduplicateEntities(details, d => d.ProductionCountries, pc => pc.Iso3166_1, ids => entity => ids.Contains(entity.Iso3166_1));
            await _deduplicationService.DeduplicateEntities(details, d => d.SpokenLanguages, sl => sl.Iso639_1, ids => entity => ids.Contains(entity.Iso639_1));
            var newProductionCompanyIds = await _deduplicationService.DeduplicateEntities(details, d => d.ProductionCompanies);
            var newMovieCollectionIds = await _deduplicationService.DeduplicateMovieCollections(details);

            await _deduplicationService.DeduplicateFilmMetadata(details);
            
            _context.FilmFiles.AddRange(newFilm);
            await _context.SaveChangesAsync();
            await _cacheClearer.Clear("OVERVIEW");
            
            foreach (var detail in details)
                _backgroundJobClient.Enqueue<ImageProcessor>(service => service.ProcessFilmDetails(detail.Id));
            foreach (var movieCollectionId in newMovieCollectionIds)
                _backgroundJobClient.Enqueue<ImageProcessor>(service => service.ProcessMovieCollection(movieCollectionId));
            if (newProductionCompanyIds.Any())
                _backgroundJobClient.Enqueue<ImageProcessor>(service => service.ProcessProductionCompanies(newProductionCompanyIds));
            foreach (var filmFile in newFilm)
                _backgroundJobClient.Enqueue<FileAnalysisService>(service => service.AnalyseFilmFile(filmFile.Id, libraryId));
        }

        [Queue(JobQueueNames.FileIndexing)]
        public async Task IndexNewEpisodes(int libraryId)
        {
            var library = await _context.Libraries.SingleAsync(l => l.Id == libraryId);
            var currentEpisodePaths = new HashSet<string>(await _context.EpisodeFiles.Where(e => e.PartOfLibraryId == libraryId).Select(e => e.Path).ToListAsync());
            var newFiles = FileIndexer.FindFiles(library.Path, "*", "mp4").Where(filePath => !currentEpisodePaths.Contains(filePath));
            var newEpisodes = FileIndexer.IndexEpisodeFiles(newFiles, library.Id).ToList();
            var details = await _metadataService.FindSeriesDetails(newEpisodes, library);
            if (!details.Any()) return;

            await _deduplicationService.DeduplicateEntities(details, d => d.Genres);
            await _deduplicationService.DeduplicateEntities(details, d => d.CreatedBy);
            var newProductionCompanyIds = await _deduplicationService.DeduplicateEntities(details, d => d.ProductionCompanies);
            var newNetworkIds = await _deduplicationService.DeduplicateEntities(details, d => d.Networks);

            await _deduplicationService.DeduplicateSeriesMetadata(details);

            _context.AddRange(newEpisodes);
            await _context.SaveChangesAsync();
            await _cacheClearer.Clear("OVERVIEW");
            
            foreach (var detail in details)
                _backgroundJobClient.Enqueue<ImageProcessor>(service => service.ProcessSeries(detail.Id));
            if (newNetworkIds.Any())
                _backgroundJobClient.Enqueue<ImageProcessor>(service => service.ProcessNetworks(newNetworkIds));
            if (newProductionCompanyIds.Any())
                _backgroundJobClient.Enqueue<ImageProcessor>(service => service.ProcessProductionCompanies(newProductionCompanyIds));
            foreach (var episodes in newEpisodes.GroupBy(e => e.SeriesDetailsId))
                _backgroundJobClient.Enqueue<FileAnalysisService>(service => service.AnalyseEpisodeFiles(episodes.Select(e => e.Id).ToArray(), libraryId));
        }
        
        [DisableConcurrentExecution(5)]
        [Queue(JobQueueNames.GenreIndexing)]
        public async Task IndexGenres(LibraryKind libraryKind, string language)
        {
            var availableGenres = libraryKind switch
            {
                LibraryKind.Film => await _metadataService.FetchFilmGenres(language),
                LibraryKind.Series => await _metadataService.FetchSeriesGenres(language),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            await _metadataService.FetchFilmGenres(language);
            var newGenreIds = availableGenres.Select(g => g.Id).ToList();
            var existingGenres = await _context.Genre.Where(g => newGenreIds.Contains(g.Id)).Select(g => g.Id).ToListAsync();
            _context.AddRange(availableGenres.Where(g => !existingGenres.Contains(g.Id)));
            await _context.SaveChangesAsync();
        }
        
        private string IndexMovieLibrary(int libraryId, string previousJob = "")
        {
            var deleteJobId = !string.IsNullOrEmpty(previousJob)
                ? _backgroundJobClient.ContinueJobWith<LibraryCleanupService>(previousJob, service => service.RemoveDeletedMovies(libraryId))
                : _backgroundJobClient.Enqueue<LibraryCleanupService>(service => service.RemoveDeletedMovies(libraryId));
            return _backgroundJobClient.ContinueJobWith<IndexingService>(deleteJobId, service => service.IndexNewMovies(libraryId));
        }
        
        private string IndexSeriesLibrary(int libraryId, string previousJob = "")
        {
            var deleteJobId = !string.IsNullOrEmpty(previousJob)
                ? _backgroundJobClient.ContinueJobWith<LibraryCleanupService>(previousJob, service => service.RemoveDeletedEpisodes(libraryId))
                : _backgroundJobClient.Enqueue<LibraryCleanupService>(service => service.RemoveDeletedEpisodes(libraryId));
            return _backgroundJobClient.ContinueJobWith<IndexingService>(deleteJobId, service => service.IndexNewEpisodes(libraryId));
        }
    }
}