namespace NxPlx.Application.Models.Events
{
    public class FileTokenRequestEvent : IEvent
    {
        public string FilePath { get; set; }
    }
}