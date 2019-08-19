using System;
using System.Collections.Generic;

namespace NxPlx.Models.File
{
    public abstract class MediaFile : PhysicalFile
    {
        public Guid MediaDetailsId { get; set; }
        
        public FFMpegProbeDetails MediaDetails { get; set; }
        
        public List<SubtitleFile> Subtitles { get; set; }
    }
}