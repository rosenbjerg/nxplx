using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NxPlx.Services.Index
{
    internal static class Utils
    {
        public static string GetSubtitleLanguage(string filepath)
        {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var index = filename.LastIndexOf('.');
            return filename.Substring(index + 1);
        }

        private static readonly Regex CueIdRegex = new Regex(@"^\d+$", RegexOptions.Compiled);
        private static readonly Regex TimeStringRegex = new Regex(@"(\d\d:\d\d:\d\d(?:[,.]\d\d\d)?) --> (\d\d:\d\d:\d\d(?:[,.]\d\d\d)?)", RegexOptions.Compiled);
        
        public static void Srt2Vtt(string srtFile, string outputFile, float offsetMs = 0)
        {
            using var vttWriter = new StreamWriter(outputFile);
            vttWriter.WriteLine("WEBVTT");
            vttWriter.WriteLine();

            foreach (var line in File.ReadLines(srtFile))
            {
                if (!CueIdRegex.IsMatch(line))
                {
                    var match = TimeStringRegex.Match(line);
                    if (match.Success)
                    {
                        var startTime = TimeSpan.Parse(match.Groups[1].Value.Replace(',', '.'));
                        var endTime =   TimeSpan.Parse(match.Groups[2].Value.Replace(',', '.'));

                        if (offsetMs != 0)
                        {
                            var startTimeMs = startTime.TotalMilliseconds + offsetMs;
                            var endTimeMs =   endTime.TotalMilliseconds + offsetMs;
                            startTime = TimeSpan.FromMilliseconds(startTimeMs < 0.0 ? 0.0 : startTimeMs);
                            endTime = TimeSpan.FromMilliseconds(endTimeMs < 0.0 ? 0.0 : endTimeMs);
                        }
                            
                        vttWriter.WriteLine($"{startTime:hh\\:mm\\:ss\\.fff} --> {endTime:hh\\:mm\\:ss\\.fff}");
                    }
                    else
                    {
                        vttWriter.WriteLine(line);
                    }
                }
            }
        }
        
    }
}