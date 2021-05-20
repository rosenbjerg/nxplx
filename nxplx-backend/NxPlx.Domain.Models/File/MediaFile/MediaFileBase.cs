using System.Collections.Generic;

namespace NxPlx.Domain.Models.File
{
    public abstract class MediaFileBase : PhysicalFileBase
    {
        public virtual MediaDetails MediaDetails { get; set; }
        
        public virtual List<SubtitleFile> Subtitles { get; set; }
    }
}