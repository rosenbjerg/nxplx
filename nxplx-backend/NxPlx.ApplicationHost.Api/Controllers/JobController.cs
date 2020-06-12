using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Mvc;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/job")]
    [ApiController]
    [SessionAuthentication]
    [RequiresAdminPermissions]
    public class JobController : ControllerBase
    {
        private IMonitoringApi _monitoringApi;

        public JobController()
        {
            _monitoringApi = Hangfire.JobStorage.Current.GetMonitoringApi();
        }

        [HttpGet("queues")]
        public IList<QueueWithTopEnqueuedJobsDto> Queues()
            => _monitoringApi.Queues();

        [HttpGet("enqueued/{queueName}/count")]
        public long EnqueuedCount([FromRoute, Required]string queueName)
            => _monitoringApi.EnqueuedCount(queueName);
        
        [HttpGet("enqueued/{queueName}")]
        public JobList<EnqueuedJobDto>Enqueued([FromRoute, Required]string queueName, [FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.EnqueuedJobs(queueName, offset ?? 0, count ?? 10);

        [HttpGet("processing/count")]
        public long ProcessingCount()
            => _monitoringApi.ProcessingCount();
        
        [HttpGet("processing")]
        public JobList<ProcessingJobDto> Processing([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.ProcessingJobs(offset ?? 0, count ?? 10);

        [HttpGet("succeeded/count")]
        public long SucceededCount()
            => _monitoringApi.SucceededListCount();
        
        [HttpGet("succeeded")]
        public JobList<SucceededJobDto> Succeeded([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.SucceededJobs(offset ?? 0, count ?? 10);

        [HttpGet("failed/count")]
        public long FailedCount()
            => _monitoringApi.FailedCount();
        
        [HttpGet("failed/{queueName}")]
        public JobList<FailedJobDto> Failed([FromQuery]int? offset, [FromQuery]int? count) 
            => _monitoringApi.FailedJobs(offset ?? 0, count ?? 10);
    }
}