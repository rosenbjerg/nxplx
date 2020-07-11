namespace NxPlx.Application.Models
{
    public class JobDto
    {
        public string Method { get; set; }
        public string Arguments { get; set; }
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