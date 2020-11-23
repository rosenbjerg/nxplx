namespace NxPlx.Application.Models
{
    public class JobQueueDto<TJobDto>
        where TJobDto : JobDto
    {
        public string Name { get; set; } = null!;
        public TJobDto[] Jobs { get; set; } = null!;
    }
}