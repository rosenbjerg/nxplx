using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services
{
    public class EditDetailsService
    {
        private readonly DatabaseContext _context;
        private readonly TempFileService _tempFileService;
        private readonly ImageCreationService _imageCreationService;
        private readonly ICacheClearer _cacheClearer;

        public EditDetailsService(DatabaseContext context, TempFileService tempFileService, ImageCreationService imageCreationService, ICacheClearer cacheClearer)
        {
            _context = context;
            _tempFileService = tempFileService;
            _imageCreationService = imageCreationService;
            _cacheClearer = cacheClearer;
        }
        public async Task<bool> SetImage(DetailsType detailsType, int detailsId, ImageType imageType, string imageExtension, Stream imageStream)
        {
            var task = detailsType switch
            {
                DetailsType.Series => SetSeriesImage(detailsId, imageType, imageExtension, imageStream),
                DetailsType.Season => SetSeasonImage(detailsId, imageType, imageExtension, imageStream),
                DetailsType.Film => SetFilmImage(detailsId, imageType, imageExtension, imageStream),
                DetailsType.Collection => SetCollectionImage(detailsId, imageType, imageExtension, imageStream),
                _ => throw new ArgumentOutOfRangeException()
            };
            var success = await task;
            if (!success) return false;
            await _context.SaveChangesAsync();
            await _cacheClearer.Clear("OVERVIEW");
            return true;
        }

        private async Task<bool> SetSeriesImage(int detailsId, ImageType imageType, string imageExtension, Stream imageStream)
        {
            var series = await _context.SeriesDetails.FirstOrDefaultAsync(sd => sd.Id == detailsId);
            if (series == null) return false;

            var tempFile = await SaveTempImage(imageExtension, imageStream);
            var task = imageType switch
            {
                ImageType.Poster => _imageCreationService.SetPoster(series, tempFile, $"{Guid.NewGuid()}.jpg"),
                ImageType.Backdrop => _imageCreationService.SetBackdrop(series, tempFile, $"{Guid.NewGuid()}.jpg"),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            await task;
            return true;
        }

        private async Task<bool> SetSeasonImage(int detailsId, ImageType imageType, string imageExtension, Stream imageStream)
        {
            var season = await _context.SeasonDetails.FirstOrDefaultAsync(sd => sd.Id == detailsId);
            if (season == null) return false;

            var tempFile = await SaveTempImage(imageExtension, imageStream);
            var task = imageType switch
            {
                ImageType.Poster => _imageCreationService.SetPoster(season, tempFile, $"{Guid.NewGuid()}.jpg"),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            await task;
            return true;
        }
        private async Task<bool> SetFilmImage(int detailsId, ImageType imageType, string imageExtension, Stream imageStream)
        {
            var film = await _context.FilmDetails.FirstOrDefaultAsync(fd => fd.Id == detailsId);
            if (film == null) return false;

            var tempFile = await SaveTempImage(imageExtension, imageStream);
            var task = imageType switch
            {
                ImageType.Poster => _imageCreationService.SetPoster(film, tempFile, $"{Guid.NewGuid()}.jpg"),
                ImageType.Backdrop => _imageCreationService.SetBackdrop(film, tempFile, $"{Guid.NewGuid()}.jpg"),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            await task;
            return true;
        }
        private async Task<bool> SetCollectionImage(int detailsId, ImageType imageType, string imageExtension, Stream imageStream)
        {
            var collection = await _context.MovieCollection.FirstOrDefaultAsync(mc => mc.Id == detailsId);
            if (collection == null) return false;

            var tempFile = await SaveTempImage(imageExtension, imageStream);
            var task = imageType switch
            {
                ImageType.Poster => _imageCreationService.SetPoster(collection, tempFile, $"{Guid.NewGuid()}.jpg"),
                ImageType.Backdrop => _imageCreationService.SetBackdrop(collection, tempFile, $"{Guid.NewGuid()}.jpg"),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            await task;
            return true;
        }

        private async Task<string> SaveTempImage(string imageExtension, Stream imageStream)
        {
            var tempFile = _tempFileService.GetFilename("image_upload", imageExtension);
            await using (var outputStream = File.OpenWrite(tempFile))
                await imageStream.CopyToAsync(outputStream);
            return tempFile;
        }
    }
}