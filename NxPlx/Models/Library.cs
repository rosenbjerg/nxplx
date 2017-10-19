using System.Collections.Generic;
using NReco.VideoInfo;

namespace NxPlx.Models
{
    class Library<TMediaFile> where TMediaFile : MediaFileBase
    {
        public string Name { get; set; }
        public LibraryType Type { get; set; }
        public string Directory { get; set; }
        public List<TMediaFile> Files { get; set; }


        public enum LibraryType
        {
            Movies,
            Series
        }
    }

    class MovieInfo
    {
        
    }

    public static class Tools
    {
        public static string[] VideoExtensions = {".mp4", ".m4v", ".webm", ".mkv", ".avi", ".ogg"};

        public static MediaInfo GetInfo(string path)
        {
            var ffProbe = new FFProbe();
            ffProbe.ToolPath = "C:\\PortableApps\\ffmpeg-3.3.3-win64-static\\bin\\ffprobe.exe";
            var info = ffProbe.GetMediaInfo(path);
            return info;
        }

    }
}