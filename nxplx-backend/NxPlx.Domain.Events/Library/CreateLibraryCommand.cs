using NxPlx.Application.Models.Jobs;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Library
{
    public class CreateLibraryCommand : IDomainCommand<AdminLibraryDto>
    {
        public CreateLibraryCommand(string name, string path, string language, string kind)
        {
            Name = name;
            Path = path;
            Language = language;
            Kind = kind;
        }
        public string Name { get; }
        public string Path { get; }
        public string Language { get; }
        public string Kind { get; }
    }
}