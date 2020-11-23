namespace NxPlx.Application.Models.Events.Library
{
    public class CreateLibraryCommand : ICommand<AdminLibraryDto>
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