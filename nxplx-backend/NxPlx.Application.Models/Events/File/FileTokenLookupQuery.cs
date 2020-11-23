namespace NxPlx.Application.Models.Events.File
{
    public class FileTokenLookupQuery : IQuery<string?>
    {
        public FileTokenLookupQuery(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}