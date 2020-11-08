namespace NxPlx.Application.Models.Events
{
    public class FileTokenLookupEvent : IEvent<string?>
    {
        public FileTokenLookupEvent(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}