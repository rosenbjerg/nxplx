using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/job")]
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

        [HttpGet("queues")]
        public IList<JobQueueDto> Queues()
            => _monitoringApi.Queues().Select(q => new JobQueueDto
            {
                Name = q.Name,
                Length = q.Length
            }).ToList();

        [HttpGet("enqueued/{queueName}/count")]
        public long EnqueuedCount([FromRoute, Required]string queueName)
            => _monitoringApi.EnqueuedCount(queueName);
        
        [HttpGet("enqueued/{queueName}")]
        public IList<JobDto> Enqueued([FromRoute, Required]string queueName, [FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.EnqueuedJobs(queueName, offset ?? 0, count ?? 10).Select(j => new JobDto
            {
                Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}",
                Arguments = string.Join(", ", j.Value.Job.Args.Select(a => a.ToString())),
                State = JobDto.JobState.Enqueued
            }).ToList();

        [HttpGet("processing/count")]
        public long ProcessingCount()
            => _monitoringApi.ProcessingCount();
        
        [HttpGet("processing")]
        public IList<JobDto> Processing([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.ProcessingJobs(offset ?? 0, count ?? 10).Select(j => new JobDto
            {
                Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}",
                Arguments = string.Join(", ", j.Value.Job.Args.Select(a => a.ToString())),
                State = JobDto.JobState.Processing
            }).ToList();

        [HttpGet("succeeded/count")]
        public long SucceededCount()
            => _monitoringApi.SucceededListCount();
        
        [HttpGet("succeeded")]
        public IList<JobDto> Succeeded([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.SucceededJobs(offset ?? 0, count ?? 10).Select(j => new JobDto
            {
                Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}",
                Arguments = string.Join(", ", j.Value.Job.Args.Select(a => a.ToString())),
                State = JobDto.JobState.Succeeded
            }).ToList();

        [HttpGet("failed/count")]
        public long FailedCount()
            => _monitoringApi.FailedCount();
        
        [HttpGet("failed/{queueName}")]
        public IList<JobDto> Failed([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.FailedJobs(offset ?? 0, count ?? 10).Select(j => new JobDto
            {
                Method = $"{j.Value.Job.Type.Name}.{j.Value.Job.Method.Name}",
                Arguments = string.Join(", ", j.Value.Job.Args.Select(a => a.ToString())),
                State = JobDto.JobState.Failed
            }).ToList();
    }
}