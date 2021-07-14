using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Services;
using NxPlx.Domain.Models;
using NxPlx.Domain.Models.File;
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

        private record ExistingFileMeta(int Id, long FileSize, DateTime LastWrite);

        [Queue(JobQueueNames.FileIndexing)]
        public async Task IndexMovies(int libraryId)
        {
            var library = await _context.Libraries.SingleAsync(l => l.Id == libraryId);
            var (newFiles, updatedFiles) = await IndexMediaFiles<FilmFile>(library);

            await IndexNewMovies(newFiles, library);

            foreach (var filmFileId in updatedFiles)
                _backgroundJobClient.Enqueue<FileAnalysisService>(service => service.AnalyseFilmFile(filmFileId, libraryId));
        }

        private async Task IndexNewMovies(string[] newFiles, Library library)
        {
            var newFilm = FileIndexer.IndexFilmFiles(newFiles, library.Id).ToList();
            var details = await _metadataService.FindFilmDetails(newFilm, library)!;
            if (details.Any())
            {
                await _deduplicationService.DeduplicateEntities(details, d => d.Genres);
                await _deduplicationService.DeduplicateEntities(details, d => d.ProductionCountries, pc => pc.Iso3166_1,
                    ids => entity => ids.Contains(entity.Iso3166_1));
                await _deduplicationService.DeduplicateEntities(details, d => d.SpokenLanguages, sl => sl.Iso639_1,
                    ids => entity => ids.Contains(entity.Iso639_1));
                var newProductionCompanyIds =
                    await _deduplicationService.DeduplicateEntities(details, d => d.ProductionCompanies);
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
                    _backgroundJobClient.Enqueue<ImageProcessor>(service =>
                        service.ProcessProductionCompanies(newProductionCompanyIds));
                foreach (var filmFile in newFilm)
                    _backgroundJobClient.Enqueue<FileAnalysisService>(
                        service => service.AnalyseFilmFile(filmFile.Id, library.Id));
            }
        }

        [Queue(JobQueueNames.FileIndexing)]
        public async Task IndexEpisodes(int libraryId)
        {
            var library = await _context.Libraries.SingleAsync(l => l.Id == libraryId);
            var (newFiles, updatedFiles) = await IndexMediaFiles<EpisodeFile>(library);
            
            await IndexNewEpisodes(newFiles, library);

            foreach (var episodeFileIds in updatedFiles.Batch(50))
                _backgroundJobClient.Enqueue<FileAnalysisService>(service => service.AnalyseEpisodeFiles(episodeFileIds, libraryId));
        }

        private async Task IndexNewEpisodes(string[] newFiles, Library library)
        {
            var newEpisodes = FileIndexer.IndexEpisodeFiles(newFiles, library.Id).ToList();
            var details = await _metadataService.FindSeriesDetails(newEpisodes, library);
            if (!details.Any())
                return;
            
            await _deduplicationService.DeduplicateEntities(details, d => d.Genres);
            await _deduplicationService.DeduplicateEntities(details, d => d.CreatedBy);
            var newProductionCompanyIds =
                await _deduplicationService.DeduplicateEntities(details, d => d.ProductionCompanies);
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
                _backgroundJobClient.Enqueue<ImageProcessor>(service =>
                    service.ProcessProductionCompanies(newProductionCompanyIds));

            foreach (var episodes in newEpisodes.GroupBy(e => e.SeriesDetailsId))
                _backgroundJobClient.Enqueue<FileAnalysisService>(service =>
                    service.AnalyseEpisodeFiles(episodes.Select(e => e.Id).ToArray(), library.Id));
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

        private async Task<(string[] newFiles, int[] updatedFiles)> IndexMediaFiles<TLibraryMember>(Library library)
            where TLibraryMember : MediaFileBase, ILibraryMember 
        {
            var currentFiles = await _context.Set<TLibraryMember>()
                .Where(f => f.PartOfLibraryId == library.Id)
                .ToDictionaryAsync(f => f.Path, f => new ExistingFileMeta(f.Id, f.FileSizeBytes, f.LastWrite));

            var newFiles = new List<string>();
            var updatedFiles = new List<int>();
            foreach (var filePath in FileIndexer.FindFiles(library.Path, "*", "mp4"))
            {
                if (!currentFiles.TryGetValue(filePath, out var found))
                    newFiles.Add(filePath);
                else
                {
                    var fileInfo = new FileInfo(filePath);
                    if (found.FileSize != fileInfo.Length || Math.Abs(fileInfo.LastWriteTimeUtc.Ticks - found.LastWrite.Ticks) > 100)
                        updatedFiles.Add(found.Id);
                }
            }

            return (newFiles.ToArray(), updatedFiles.ToArray());
        }
        
        private string IndexMovieLibrary(int libraryId, string previousJob = "")
        {
            var deleteJobId = !string.IsNullOrEmpty(previousJob)
                ? _backgroundJobClient.ContinueJobWith<LibraryCleanupService>(previousJob, service => service.RemoveDeletedMovies(libraryId))
                : _backgroundJobClient.Enqueue<LibraryCleanupService>(service => service.RemoveDeletedMovies(libraryId));
            return _backgroundJobClient.ContinueJobWith<IndexingService>(deleteJobId, service => service.IndexMovies(libraryId));
        }
        
        private string IndexSeriesLibrary(int libraryId, string previousJob = "")
        {
            var deleteJobId = !string.IsNullOrEmpty(previousJob)
                ? _backgroundJobClient.ContinueJobWith<LibraryCleanupService>(previousJob, service => service.RemoveDeletedEpisodes(libraryId))
                : _backgroundJobClient.Enqueue<LibraryCleanupService>(service => service.RemoveDeletedEpisodes(libraryId));
            return _backgroundJobClient.ContinueJobWith<IndexingService>(deleteJobId, service => service.IndexEpisodes(libraryId));
        }
    }
}