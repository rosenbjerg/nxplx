using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using FailedJobDto = NxPlx.Application.Models.FailedJobDto;
using SucceededJobDto = NxPlx.Application.Models.SucceededJobDto;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    [SessionAuthentication]
    [RequiresAdminPermissions]
    public class JobController : ControllerBase
    {
        private readonly IMonitoringApi _monitoringApi;
        public JobController()
        {
            _monitoringApi = Hangfire.JobStorage.Current.GetMonitoringApi();
        }

        [HttpGet("enqueued/count")]
        public long EnqueuedCount() => _monitoringApi.Queues().Sum(queue => _monitoringApi.EnqueuedCount(queue.Name));
        
        [HttpGet("processing/count")]
        public long ProcessingCount() => _monitoringApi.ProcessingCount();
        
        [HttpGet("failed/count")]
        public long FailedCount() => _monitoringApi.FailedCount();
        
        [HttpGet("succeeded/count")]
        public long SucceededCount() => _monitoringApi.SucceededListCount();
        
        [HttpGet("enqueued")]
        public JobQueueDto<JobDto>[] Enqueued([FromRoute, Required]string queueName, [FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.Queues().Select(queue => new JobQueueDto<JobDto>
            {
                Name = queue.Name,
                Jobs = _monitoringApi.EnqueuedJobs(queueName, offset ?? 0, count ?? 25).Select(j => new JobDto
                {
                    Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}({string.Join(", ", j.Value.Job.Args.Select(a => a.ToString()))})",
                }).ToArray()
            }).ToArray();
        
        [HttpGet("processing")]
        public JobDto[] Processing([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.ProcessingJobs(offset ?? 0, count ?? 25).Select(j => new JobDto
            {
                Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}({string.Join(", ", j.Value.Job.Args.Select(a => a.ToString()))})",
            }).ToArray();
        
        [HttpGet("failed")]
        public JobQueueDto<FailedJobDto>[] Failed([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.Queues().Select(queue => new JobQueueDto<FailedJobDto>
            {
                Name = queue.Name,
                Jobs = _monitoringApi.FailedJobs(offset ?? 0, count ?? 25).Select(j => new FailedJobDto
                {
                    Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}({string.Join(", ", j.Value.Job.Args.Select(a => a.ToString()))})",
                    ExceptionType = j.Value.ExceptionType,
                    ExceptionMessage = j.Value.ExceptionMessage,
                    ExceptionDetails = j.Value.ExceptionDetails,
                }).ToArray()
            }).ToArray();
        
        [HttpGet("succeeded")]
        public SucceededJobDto[] Succeeded([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.SucceededJobs(offset ?? 0, count ?? 25).Select(j => new SucceededJobDto
            {
                Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}({string.Join(", ", j.Value.Job.Args.Select(a => a.ToString()))})",
                ExecutionTime = j.Value.TotalDuration ?? -1
            }).ToArray();
    }
}