﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Infrastructure.Database;
using Z.EntityFramework.Plus;

namespace NxPlx.Services.Index
{
    public class LibraryCleanupService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ICacheClearer _cacheClearer;
        private readonly ILogger<LibraryCleanupService> _logger;

        public LibraryCleanupService(DatabaseContext databaseContext, ICacheClearer cacheClearer, ILogger<LibraryCleanupService> logger)
        {
            _databaseContext = databaseContext;
            _cacheClearer = cacheClearer;
            _logger = logger;
        }
        [Queue(JobQueueNames.FileIndexing)]
        public async Task RemoveDeletedMovies(int libraryId)
        {
            var currentFilePaths = await _databaseContext.FilmFiles.Where(f => f.PartOfLibraryId == libraryId).Select(e => new { e.Id, e.Path }).ToListAsync();
            var deletedIds = currentFilePaths.Where(file => !File.Exists(file.Path)).Select(file => file.Id).ToList();
            if (deletedIds.Any())
            {
                await RemoveWatchingProgress(deletedIds);
                await RemoveSubtitlePreferences(deletedIds);
                var deleted = await _databaseContext.FilmFiles.Where(f => deletedIds.Contains(f.Id)).DeleteAsync();
                await _databaseContext.SaveChangesAsync();
                await _cacheClearer.Clear("OVERVIEW");
                _logger.LogInformation("Deleted {DeletedAmount} film from Library {LibaryId} because files were removed", deleted, libraryId);
            }
        }
        
        [Queue(JobQueueNames.FileIndexing)]
        public async Task RemoveDeletedEpisodes(int libraryId)
        {
            var currentFilePaths = await _databaseContext.EpisodeFiles.Where(f => f.PartOfLibraryId == libraryId).Select(e => new { e.Id, e.Path }).ToListAsync();
            var deletedIds = currentFilePaths.Where(file => !File.Exists(file.Path)).Select(file => file.Id).ToList();
            if (deletedIds.Any())
            {
                await RemoveWatchingProgress(deletedIds);
                await RemoveSubtitlePreferences(deletedIds);
                var deleted = await _databaseContext.EpisodeFiles.Where(f => deletedIds.Contains(f.Id)).DeleteAsync();
                await _databaseContext.SaveChangesAsync();
                await _cacheClearer.Clear("OVERVIEW");
                _logger.LogInformation("Deleted {DeletedAmount} film from Library {LibaryId} because files were removed", deleted, libraryId);
            }
        }
        
        private async Task RemoveWatchingProgress(List<int> fileIds)
        {
            var progress = await _databaseContext.WatchingProgresses.Where(wp => fileIds.Contains(wp.FileId)).ToListAsync();
            _databaseContext.WatchingProgresses.RemoveRange(progress);
        }
        private async Task RemoveSubtitlePreferences(List<int> fileIds)
        {
            var progress = await _databaseContext.SubtitlePreferences.Where(sp => fileIds.Contains(sp.FileId)).ToListAsync();
            _databaseContext.SubtitlePreferences.RemoveRange(progress);
        }
    }
}