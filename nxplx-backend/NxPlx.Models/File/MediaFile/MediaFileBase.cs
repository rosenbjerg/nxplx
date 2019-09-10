using System.Collections.Generic;

namespace NxPlx.Models.File
{
    public abstract class MediaFileBase : PhysicalFileBase
    {
        public int MediaDetailsId { get; set; }
        
        public FFMpegProbeDetails MediaDetails { get; set; }
        
        public List<SubtitleFile> Subtitles { get; set; }
    }
}