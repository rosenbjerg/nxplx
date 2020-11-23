using System.Collections.Generic;
using NxPlx.Models;

namespace NxPlx.Application.Models.Events.Series
{
    public class AvailableSubtitleLanguageQuery : IQuery<List<string>>
    {
        public AvailableSubtitleLanguageQuery(MediaFileType mediaType, int fileId)
        {
            MediaType = mediaType;
            FileId = fileId;
        }

        public MediaFileType MediaType { get; }
        public int FileId { get; }
    }
}