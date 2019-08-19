using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Models.File;
using NxPlx.Services.Database;

namespace NxPlx.Services.Index
{
    public class Indexer
    {
        public async Task<List<SubtitleFile>> IndexSubtitles(string filepath)
        {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var srtFiles = Directory.GetFiles(Path.GetDirectoryName(filepath), $"{filename}.*.srt")
                .Concat(Directory.GetFiles(Path.GetDirectoryName(filepath), $"{filename}.*.vtt"))
                .Select(srt =>
                {
                    var vtt = srt.Replace(".srt", ".vtt");
                    var vttHash = vtt.GetHashCode();
                    return (srt, vtt, vttHash);
                });
            
            var subtitles = new List<SubtitleFile>();
            foreach (var (srt, vtt, hash) in srtFiles)
            {                
                string language;
                if (File.Exists(vtt))
                {
                    language = Utils.GetSubtitleLanguage(vtt);
                }
                else
                {
                    await Utils.Srt2Vtt(srt, vtt);
                    language = Utils.GetSubtitleLanguage(vtt);
                }

                var fileInfo = new FileInfo(vtt);
                subtitles.Add(new SubtitleFile
                {
                    Path = vtt,
                    PathHash = hash,
                    Language = language,
                    FileSizeBytes = fileInfo.Length,
                    Added = DateTime.UtcNow,
                    Created = fileInfo.CreationTimeUtc,
                    LastWrite = fileInfo.LastWriteTimeUtc
                });
            }
            
            return subtitles;
        }

        private Regex _seriesRegex = new Regex("^(?<name>.+?)??[ -]*([Ss](?<season>\\d{1,3}))? ?[Ee](?<episode>\\d{1,3})", RegexOptions.Compiled);
        private Regex _filmRegex = new Regex("^(?<title>.+?)??\\(?(?<year>\\d{4})\\)?[ .]?", RegexOptions.Compiled);
        private Regex _whitespaceRegex = new Regex("[\\s.-]", RegexOptions.Compiled);
        
        public async Task IndexFolder(string folder)
        {
            var start = DateTime.UtcNow;
            var mp4Files = Directory.GetFiles(folder, "*.mp4", SearchOption.AllDirectories);
            var episodesPaths = mp4Files.Where(mp4 => _seriesRegex.IsMatch(Path.GetFileNameWithoutExtension(mp4))).ToArray();
            var filmPaths = mp4Files.Except(episodesPaths).ToArray();
            
            Console.WriteLine($"finding took {DateTime.UtcNow.Subtract(start).TotalMilliseconds}ms");
            start = DateTime.UtcNow;
            
            var episodes = IndexEpisodes(episodesPaths);
            var film = IndexFilm(filmPaths);
            
            Console.WriteLine($"indexing took {DateTime.UtcNow.Subtract(start).TotalMilliseconds}ms");
            start = DateTime.UtcNow;

//            using (var ctx = new MediaContext())
//            {
//                ctx.BulkInsert(episodes);
//                await ctx.AddRangeAsync(episodes);
//                await ctx.AddRangeAsync(film);
//                await ctx.SaveChangesAsync();
//            }
//            Console.WriteLine($"saving took {DateTime.UtcNow.Subtract(start).TotalMilliseconds}ms");
            
            Console.WriteLine();
        }

        private List<EpisodeFile> IndexEpisodes(IEnumerable<string> episodePaths)
        {
            return (from episodePath in episodePaths.AsParallel().WithDegreeOfParallelism(4)
                let fileInfo = new FileInfo(episodePath)
                let match = _seriesRegex.Match(Path.GetFileNameWithoutExtension(episodePath))
                let nameGroup = match.Groups["name"]
                let seasonGroup = match.Groups["season"]
                let episodeGroup = match.Groups["episode"]
                select new EpisodeFile
                {
                    Added = DateTime.UtcNow,
                    Created = fileInfo.CreationTimeUtc,
                    LastWrite = fileInfo.LastWriteTimeUtc,
                    Name = TitleCleanup(nameGroup.Value),
                    SeasonNumber = seasonGroup.Success ? int.Parse(seasonGroup.Value) : 0,
                    EpisodeNumber = episodeGroup.Success ? int.Parse(episodeGroup.Value) : 0,
                    Path = episodePath,
                    PathHash = episodePaths.GetHashCode(),
                    FileSizeBytes = fileInfo.Length,
                    Subtitles = new List<SubtitleFile>(),
                }).ToList();
        }
        
        private List<FilmFile> IndexFilm(IEnumerable<string> filmPaths)
        {
            return (from filmPath in filmPaths.AsParallel().WithDegreeOfParallelism(4)
                let fileInfo = new FileInfo(filmPath)
                let match = _filmRegex.Match(Path.GetFileNameWithoutExtension(filmPath))
                let titleGroup = match.Groups["title"]
                let yearGroup = match.Groups["year"]
                select new FilmFile
                {
                    Added = DateTime.UtcNow,
                    Created = fileInfo.CreationTimeUtc,
                    LastWrite = fileInfo.LastWriteTimeUtc,
                    Title = TitleCleanup(titleGroup.Value),
                    Year = yearGroup.Success ? int.Parse(yearGroup.Value) : 0,
                    Path = filmPath,
                    PathHash = filmPath.GetHashCode(),
                    FileSizeBytes = fileInfo.Length,
                    Subtitles = new List<SubtitleFile>(),
                }).ToList();
        }

        public string TitleCleanup(string input)
        {
            return _whitespaceRegex.Replace(input, " ").Trim(' ', '.', '-');
        }
        
    }
}