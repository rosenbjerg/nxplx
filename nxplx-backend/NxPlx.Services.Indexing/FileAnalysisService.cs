using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Infrastructure.Database;
using NxPlx.Domain.Models.File;

namespace NxPlx.Services.Index
{
    public class FileAnalysisService
    {
        private readonly DatabaseContext _databaseContext;

        public FileAnalysisService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        [Queue(JobQueueNames.FileAnalysis)]
        public async Task AnalyseFilmFile(int filmFileId, int libraryId)
        {
            var filmFile = await _databaseContext.FilmFiles.SingleAsync(ff => ff.Id == filmFileId);
            var fileInfo = new FileInfo(filmFile.Path);
            filmFile.LastWrite = fileInfo.LastWriteTimeUtc;
            filmFile.FileSizeBytes = fileInfo.Length;
            filmFile.PartOfLibraryId = libraryId;
            filmFile.MediaDetails = await AnalyseMedia(filmFile.Path);
            filmFile.Subtitles = FileIndexer.IndexSubtitles(filmFile.Path);
            await _databaseContext.SaveChangesAsync();
        }
        
        [Queue(JobQueueNames.FileAnalysis)]
        public async Task AnalyseEpisodeFiles(IReadOnlyCollection<int> episodeFileIds, int libraryId)
        {
            var episodeFiles = await _databaseContext.EpisodeFiles.Where(e => episodeFileIds.Contains(e.Id)).ToListAsync();
            foreach (var episodeFile in episodeFiles)
            {
                var fileInfo = new FileInfo(episodeFile.Path);
                episodeFile.LastWrite = fileInfo.LastWriteTimeUtc;
                episodeFile.FileSizeBytes = fileInfo.Length;
                episodeFile.PartOfLibraryId = libraryId;
                episodeFile.MediaDetails = await AnalyseMedia(episodeFile.Path);
                episodeFile.Subtitles = FileIndexer.IndexSubtitles(episodeFile.Path);
            }
            await _databaseContext.SaveChangesAsync();
        }
        
        private static async Task<MediaDetails> AnalyseMedia(string path)
        {
            var analysis = await FFMpegCore.FFProbe.AnalyseAsync(path);
            return new MediaDetails
            {
                Duration = (float)analysis!.Duration.TotalSeconds,
                AudioBitrate = analysis.PrimaryAudioStream?.BitRate ?? 0,
                AudioCodec = analysis.PrimaryAudioStream?.CodecName ?? "-",
                AudioChannelLayout = analysis.PrimaryAudioStream?.ChannelLayout ?? "-",
                AudioStreamIndex = analysis.PrimaryAudioStream?.Index ?? -1,
                VideoBitrate = analysis.PrimaryVideoStream!.BitRate,
                VideoCodec = analysis.PrimaryVideoStream.CodecName,
                VideoHeight = analysis.PrimaryVideoStream.Height,
                VideoWidth = analysis.PrimaryVideoStream.Width,
                VideoAspectRatio = $"{analysis.PrimaryVideoStream.DisplayAspectRatio.Width}:{analysis.PrimaryVideoStream.DisplayAspectRatio.Height}",
                VideoBitDepth = analysis.PrimaryVideoStream.BitsPerRawSample,
                VideoFrameRate = (float)analysis.PrimaryVideoStream.FrameRate
            };
        }
    }
}