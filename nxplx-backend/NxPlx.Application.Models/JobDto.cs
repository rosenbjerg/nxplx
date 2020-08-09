namespace NxPlx.Application.Models
{
    public class JobDto
    {
        public string Method { get; set; } = null!;
    }

    public class SucceededJobDto : JobDto
    {
        public long ExecutionTime { get; set; }
    }
    public class FailedJobDto : JobDto
    {
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionDetails { get; set; }
    }

    public class JobQueueDto<TJobDto>
        where TJobDto : JobDto
    {
        public string Name { get; set; }
        public TJobDto[] Jobs { get; set; }
    }
}