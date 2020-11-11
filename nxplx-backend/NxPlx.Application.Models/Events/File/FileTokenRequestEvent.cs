namespace NxPlx.Application.Models.Events
{
    public class FileTokenRequestEvent : IEvent<string>
    {
        public FileTokenRequestEvent(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}