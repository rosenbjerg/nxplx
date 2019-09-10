using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NxPlx.Models.File;

namespace NxPlx.Services.Index
{
    public class FileIndexer
    {
        private IEnumerable<SubtitleFile> IndexSubtitles(string filepath)
        {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            return GetAllFiles(Path.GetDirectoryName(filepath), $"*{filename}.*", "srt", "vtt")
                .Select(subtitleFile =>
                {
                    var vtt = subtitleFile.Replace(".srt", ".vtt");
                    string language;
                    if (File.Exists(vtt))
                    {
                        language = Utils.GetSubtitleLanguage(vtt);
                    }
                    else
                    {
                        Utils.Srt2Vtt(subtitleFile, vtt);
                        language = Utils.GetSubtitleLanguage(vtt);
                    }

                    var fileInfo = new FileInfo(vtt);
                    return new SubtitleFile
                    {
                        Path = vtt,
                        Language = language,
                        FileSizeBytes = fileInfo.Length,
                        Added = DateTime.UtcNow,
                        Created = fileInfo.CreationTimeUtc,
                        LastWrite = fileInfo.LastWriteTimeUtc
                    };
                });
        }
        private static IEnumerable<string> GetAllFiles(string folder, string pattern, params string[] extensions)
        {
            return extensions.SelectMany(ext => Directory.GetFiles(folder, $"{pattern}.{ext}", SearchOption.AllDirectories));
        }

        private Regex _seriesRegex = new Regex("^(?<name>.+?)??[ -]*([Ss](?<season>\\d{1,3}))? ?[Ee](?<episode>\\d{1,3})", RegexOptions.Compiled);
        private Regex _filmRegex = new Regex("^(?<title>.+?)?\\(?(?<year>\\d{4})\\)??[ .]?", RegexOptions.Compiled);
        private Regex _whitespaceRegex = new Regex("[\\s\\.-]+", RegexOptions.Compiled);
        
        public List<EpisodeFile> IndexEpisodes(HashSet<string> existing, IEnumerable<string> folders)
        {
            return folders.SelectMany(folder =>
                {
                    var mp4Files = Directory.GetFiles(folder, "*.mp4", SearchOption.AllDirectories);
                    return mp4Files
                        .Where(mp4 =>
                            !existing.Contains(mp4) && _seriesRegex.IsMatch(Path.GetFileNameWithoutExtension(mp4)))
                        .ToArray();
                })
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Select(episodePath =>
                {
                    var fileInfo = new FileInfo(episodePath);
                    var match = _seriesRegex.Match(Path.GetFileNameWithoutExtension(episodePath));
                    var subtitles = IndexSubtitles(episodePath).ToList();
                    var nameGroup = match.Groups["name"];
                    var seasonGroup = match.Groups["season"];
                    var episodeGroup = match.Groups["episode"];

                    return new EpisodeFile
                    {
                        Added = DateTime.UtcNow,
                        Created = fileInfo.CreationTimeUtc,
                        LastWrite = fileInfo.LastWriteTimeUtc,
                        Name = TitleCleanup(nameGroup.Value),
                        SeasonNumber = seasonGroup.Success ? int.Parse(seasonGroup.Value) : 0,
                        EpisodeNumber = episodeGroup.Success ? int.Parse(episodeGroup.Value) : 0,
                        Path = episodePath,
                        FileSizeBytes = fileInfo.Length,
                        Subtitles = subtitles,
                        MediaDetails = new FFMpegProbeDetails()
                    };
                }).ToList();
        }
        
        public List<FilmFile> IndexFilm(HashSet<string> existing, IEnumerable<string> folders)
        {
            return folders.SelectMany(folder =>
                {
                    var mp4Files = Directory.GetFiles(folder, "*.mp4", SearchOption.AllDirectories);
                    return mp4Files
                        .Where(mp4 =>
                            !existing.Contains(mp4) && !_seriesRegex.IsMatch(Path.GetFileNameWithoutExtension(mp4)))
                        .ToArray();
                })
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Select(filmPath =>
                {
                    var fileInfo = new FileInfo(filmPath);
                    var match = _filmRegex.Match(Path.GetFileNameWithoutExtension(filmPath));
                    var titleGroup = match.Groups["title"];
                    var yearGroup = match.Groups["year"];

                    var title = titleGroup.Value != "" ? titleGroup.Value : Path.GetFileNameWithoutExtension(filmPath);

                    return new FilmFile
                    {
                        Added = DateTime.UtcNow,
                        Created = fileInfo.CreationTimeUtc,
                        LastWrite = fileInfo.LastWriteTimeUtc,
                        Title = TitleCleanup(title),
                        Year = yearGroup.Success ? int.Parse(yearGroup.Value) : 1,
                        Path = filmPath,
                        FileSizeBytes = fileInfo.Length,
                        Subtitles = IndexSubtitles(filmPath).ToList(),
                        MediaDetails = new FFMpegProbeDetails()
                    };
                }).ToList();
        }

        private string TitleCleanup(string input)
        {
            var step1 = _whitespaceRegex.Replace(input, " ").Trim(' ');
            if (string.IsNullOrEmpty(step1)) return step1;

            var step2 = step1
                .Split(" ")
                .Select(word => word[0].ToString().ToUpperInvariant() + word.Substring(1).ToLowerInvariant());
            
            return string.Join(" ", step2);
        }
        
    }
}