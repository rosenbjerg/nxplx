using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nxplx.Integrations.FFMpeg;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Services.Index
{
    public class FileIndexer
    {
        private List<SubtitleFile> IndexSubtitles(string filepath)
        {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var files = FindFiles(Path.GetDirectoryName(filepath), $"*{filename}.*", "srt", "vtt");
            var uniqueSubs = new Dictionary<string, SubtitleFile>();
            
            foreach (var file in files)
            {
                var vtt = file.Replace(".srt", ".vtt");
                
                if (uniqueSubs.ContainsKey(vtt))
                    continue;
                
                string language;
                if (File.Exists(vtt))
                {
                    language = Utils.GetSubtitleLanguage(vtt);
                }
                else
                {
                    Utils.Srt2Vtt(file, vtt);
                    language = Utils.GetSubtitleLanguage(vtt);
                }

                var fileInfo = new FileInfo(vtt);
                uniqueSubs[vtt] = new SubtitleFile
                {
                    Path = vtt,
                    Language = language,
                    FileSizeBytes = fileInfo.Length,
                    Added = DateTime.UtcNow,
                    Created = fileInfo.CreationTimeUtc,
                    LastWrite = fileInfo.LastWriteTimeUtc
                };
            }

            return uniqueSubs.Values.ToList();
        }
        private static IEnumerable<string> FindFiles(string folder, string pattern, params string[] extensions)
        {
            return extensions.SelectMany(ext => Directory.EnumerateFiles(folder, $"{pattern}.{ext}", SearchOption.AllDirectories));
        }
        
        private readonly Regex _seriesRegex = new Regex("^(?<name>.+?)??[ -]*([Ss](?<season>\\d{1,3}))? ?[Ee](?<episode>\\d{1,3})", RegexOptions.Compiled);
        private readonly Regex _filmRegex = new Regex("^(?<title>.+)?\\(?(?<year>\\d{4})\\)??[ .]?", RegexOptions.Compiled);
        private readonly Regex _whitespaceRegex = new Regex("[\\s\\.-]+", RegexOptions.Compiled);

        private readonly string[] StopWords = { "(", ")", "1080", "1440", "2160", "4096", "4320", "8192" };
        
        public List<EpisodeFile> IndexEpisodes(HashSet<string> existing, Library library)
        {
            var newFiles = FindFiles(library.Path, "*", "mp4").Where(filePath => !existing.Contains(filePath));
            var episodes = IndexEpisodeFiles(newFiles);

            return episodes
                .AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount * 3)
                .Select(episode =>
                {
                    var fileInfo = new FileInfo(episode.Path);
                    episode.Created = fileInfo.CreationTimeUtc;
                    episode.LastWrite = fileInfo.LastWriteTimeUtc;
                    episode.FileSizeBytes = fileInfo.Length;

                    episode.PartOfLibraryId = library.Id;
                    episode.MediaDetails = FFProbe.Analyse(episode.Path);
                    episode.Subtitles = IndexSubtitles(episode.Path);
                    
                    return episode;
                }).ToList();
        }

        public IEnumerable<EpisodeFile> IndexEpisodeFiles(IEnumerable<string> filesPath)
        {
            return filesPath
                .Where(mp4 => _seriesRegex.IsMatch(Path.GetFileNameWithoutExtension(mp4)))
                .Select(episodePath =>
            {
                var match = _seriesRegex.Match(Path.GetFileNameWithoutExtension(episodePath));
                var nameGroup = match.Groups["name"];
                var seasonGroup = match.Groups["season"];
                var episodeGroup = match.Groups["episode"];
                var name = nameGroup.Value != "" ? nameGroup.Value : Path.GetFileNameWithoutExtension(episodePath);

                return new EpisodeFile
                {
                    Added = DateTime.UtcNow,
                    Name = TitleCleanup(name),
                    SeasonNumber = seasonGroup.Success ? int.Parse(seasonGroup.Value) : 1,
                    EpisodeNumber = episodeGroup.Success ? int.Parse(episodeGroup.Value) : 0,
                    Path = episodePath
                };
            });
        }
        
        public List<FilmFile> IndexFilm(HashSet<string> existing, Library library)
        {
            var newFiles = FindFiles(library.Path, "*", "mp4").Where(filePath => !existing.Contains(filePath));
            var newFilm = IndexFilmFiles(newFiles);

            return newFilm
                .AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount * 3)
                .Select(film =>
                {
                    var fileInfo = new FileInfo(film.Path);
                    film.Created = fileInfo.CreationTimeUtc;
                    film.LastWrite = fileInfo.LastWriteTimeUtc;
                    film.FileSizeBytes = fileInfo.Length;

                    film.PartOfLibraryId = library.Id;
                    film.MediaDetails = FFProbe.Analyse(film.Path);
                    film.Subtitles = IndexSubtitles(film.Path);

                    return film;
                }).ToList();
        }

        public IEnumerable<FilmFile> IndexFilmFiles(IEnumerable<string> filesPath)
        {
            return filesPath
                .Where(mp4 => !_seriesRegex.IsMatch(Path.GetFileNameWithoutExtension(mp4)))
                .Select(filmPath =>
                {
                    var filename = Path.GetFileNameWithoutExtension(filmPath);
                    var trimmedName = StopWords.Aggregate(filename, (acc, stopword) => acc.Replace(stopword, ""));
                    var match = _filmRegex.Match(trimmedName);
                    var titleGroup = match.Groups["title"];
                    var yearGroup = match.Groups["year"];
                    var title = titleGroup.Value != "" ? titleGroup.Value : Path.GetFileNameWithoutExtension(trimmedName);

                    return new FilmFile
                    {
                        Added = DateTime.UtcNow,
                        Title = TitleCleanup(title),
                        Year = yearGroup.Success ? int.Parse(yearGroup.Value) : 1,
                        Path = filmPath,
                    };
                });
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