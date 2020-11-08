namespace NxPlx.Application.Models.Events
{
    public class FileTokenLookupEvent : IEvent
    {
        public string Token { get; set; }
    }
}