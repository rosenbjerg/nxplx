using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NxPlx.Domain.Models;
using NxPlx.Domain.Models.File;

namespace NxPlx.Services.Index
{
    public class FileIndexer
    {
        public static List<SubtitleFile> IndexSubtitles(string filepath)
        {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var files = FindFiles(Path.GetDirectoryName(filepath)!, $"*{filename}.*", "srt", "vtt");
            var uniqueSubs = new Dictionary<string, SubtitleFile>();
            
            foreach (var file in files)
            {
                var vtt = file.Replace(".srt", ".vtt");
                
                if (uniqueSubs.ContainsKey(vtt))
                    continue;

                if (!File.Exists(vtt)) Utils.Srt2Vtt(file, vtt);
                var language = Utils.GetSubtitleLanguage(vtt);

                var fileInfo = new FileInfo(vtt);
                uniqueSubs[vtt] = new SubtitleFile
                {
                    Path = vtt,
                    Language = language,
                    FileSizeBytes = fileInfo.Length,
                    LastWrite = fileInfo.LastWriteTimeUtc
                };
            }

            return uniqueSubs.Values.ToList();
        }

        public static IEnumerable<string> FindFiles(string folder, string pattern, params string[] extensions)
        {
            return extensions.SelectMany(ext => Directory.EnumerateFiles(folder, $"{pattern}.{ext}", SearchOption.AllDirectories));
        }
        
        private static readonly Regex SeriesRegex = new("^(?<name>.+?)??[ -]*([Ss]?(?<season>\\d{1,3}))? ?[Eex](?<episode>\\d{1,3})", RegexOptions.Compiled);
        private static readonly Regex FilmRegex = new("^(?<title>.+)?\\(?(?<year>\\d{4})\\)??[ .]?", RegexOptions.Compiled);
        private static readonly Regex WhitespaceRegex = new("[\\s\\.-]+", RegexOptions.Compiled);

        private static readonly string[] StopWords = { "(", ")", "1080", "1440", "2160", "4096", "4320", "8192" };
        
        public static IEnumerable<EpisodeFile> IndexEpisodeFiles(IEnumerable<string> filesPath, int libraryId)
        {
            return filesPath
                .Where(mp4 => SeriesRegex.IsMatch(Path.GetFileNameWithoutExtension(mp4)))
                .Select(episodePath =>
            {
                var match = SeriesRegex.Match(Path.GetFileNameWithoutExtension(episodePath));
                var nameGroup = match.Groups["name"];
                var seasonGroup = match.Groups["season"];
                var episodeGroup = match.Groups["episode"];
                var name = nameGroup.Value != "" ? nameGroup.Value : Path.GetFileNameWithoutExtension(episodePath);

                return new EpisodeFile
                {
                    Name = TitleCleanup(name),
                    SeasonNumber = seasonGroup.Success ? int.Parse(seasonGroup.Value) : 1,
                    EpisodeNumber = episodeGroup.Success ? int.Parse(episodeGroup.Value) : 0,
                    Path = episodePath,
                    PartOfLibraryId = libraryId
                };
            });
        }
        
        public static IEnumerable<FilmFile> IndexFilmFiles(IEnumerable<string> filesPath, int libraryId)
        {
            return filesPath
                .Where(mp4 => !SeriesRegex.IsMatch(Path.GetFileNameWithoutExtension(mp4)))
                .Select(filmPath =>
                {
                    var filename = Path.GetFileNameWithoutExtension(filmPath);
                    var trimmedName = StopWords.Aggregate(filename, (acc, stopword) => acc.Replace(stopword, ""));
                    var match = FilmRegex.Match(trimmedName);
                    var titleGroup = match.Groups["title"];
                    var yearGroup = match.Groups["year"];
                    var title = titleGroup.Value != "" ? titleGroup.Value : Path.GetFileNameWithoutExtension(trimmedName);

                    return new FilmFile
                    {
                        Title = TitleCleanup(title),
                        Path = Path.GetFullPath(filmPath),
                        Year = yearGroup.Success ? int.Parse(yearGroup.Value) : 1,
                        PartOfLibraryId = libraryId
                    };
                });
        }
        private static string TitleCleanup(string input)
        {
            var step1 = WhitespaceRegex.Replace(input, " ").Trim(' ');
            if (string.IsNullOrEmpty(step1)) return step1;

            var step2 = step1
                .Split(" ")
                .Select(word => word[0].ToString().ToUpperInvariant() + word.Substring(1).ToLowerInvariant());
            
            return string.Join(" ", step2);
        }
    }
}