namespace NxPlx.Application.Core.Options
{
    public class ApiDocumentationOptions : INxplxOptions
    {
        public bool Enabled { get; set; } = false;
        public string PathPrefix { get; set; } = null!;
    }
}