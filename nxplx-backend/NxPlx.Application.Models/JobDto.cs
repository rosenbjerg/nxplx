namespace NxPlx.Application.Models
{
    public class JobDto
    {
        public string Method { get; set; } = null!;
        public string Arguments { get; set; } = null!;
        public JobState State { get; set; }
            
        public enum JobState
        {
            Enqueued,
            Processing,
            Succeeded,
            Failed
        }
    }
}