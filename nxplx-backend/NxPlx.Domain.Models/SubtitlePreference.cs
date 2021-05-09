namespace NxPlx.Domain.Models
{
    public class SubtitlePreference
    {
        public int UserId { get; set; }
        public int FileId { get; set; }
        public string Language { get; set; }
        public MediaFileType MediaType { get; set; }
    }
}