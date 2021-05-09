namespace NxPlx.Application.Models.Jobs
{
    public class FailedJobDto : JobDto
    {
        public string ExceptionType { get; set; } = null!;
        public string ExceptionMessage { get; set; } = null!;
        public string ExceptionDetails { get; set; } = null!;
    }
}