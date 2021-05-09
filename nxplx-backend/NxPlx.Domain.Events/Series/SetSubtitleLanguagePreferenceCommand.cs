using System.Threading.Tasks;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class SetSubtitleLanguagePreferenceCommand : IDomainCommand<Task>
    {
        public SetSubtitleLanguagePreferenceCommand(MediaFileType mediaType, int fileId, string language)
        {
            MediaType = mediaType;
            FileId = fileId;
            Language = language;
        }

        public MediaFileType MediaType { get; }
        public int FileId { get; }
        public string Language { get; }
    }
}