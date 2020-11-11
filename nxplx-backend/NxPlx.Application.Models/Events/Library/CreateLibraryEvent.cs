namespace NxPlx.Application.Models.Events
{
    public class CreateLibraryEvent : IEvent<AdminLibraryDto>
    {
        public CreateLibraryEvent(string name, string path, string language, string kind)
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