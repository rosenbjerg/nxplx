namespace NxPlx.Application.Models.Events.File
{
    public class RequestFileTokenCommand : ICommand<string>
    {
        public RequestFileTokenCommand(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}