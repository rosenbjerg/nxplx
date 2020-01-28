using System;

namespace NxPlx.Models
{
    public class WatchingProgress
    {
        public int UserId { get; set; }
        public MediaFileType MediaType { get; set; }
        public int FileId { get; set; }

        public DateTime LastWatched { get; set; }
        public double Time { get; set; }
    }
}