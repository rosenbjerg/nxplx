using System;

namespace NxPlx.Models.File
{
    public class SubtitleFile : PhysicalFile
    {
        public string Language { get; set; }

        public Guid MediaFileId { get; set; }
        
        public MediaFile MediaFile { get; set; }
    }
}