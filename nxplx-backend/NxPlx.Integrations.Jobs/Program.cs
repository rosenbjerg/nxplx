using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using NxPlx.Configuration;

namespace NxPlx.Jobs.Integration
{
    public static class JobService
    {
        [QueueAttribute("convert")]
        public static string Enqueue(Expression<Action> job, int retryAttempts = 0)
        {
            var cfg = ConfigurationService.Current;
            var connectionString = $"Host={cfg.SqlHost};Database={cfg.SqlJobDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}";
            var client = new BackgroundJobClient(new PostgreSqlStorage(connectionString))
            {
                RetryAttempts = retryAttempts
            };
            return client.Enqueue(job);
        }
        
        [QueueAttribute("convert")]
        public static string Enqueue(Expression<Func<Task>> job, int retryAttempts = 0)
        {
            var cfg = ConfigurationService.Current;
            var connectionString = $"Host={cfg.SqlHost};Database={cfg.SqlJobDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}";
            var client = new BackgroundJobClient(new PostgreSqlStorage(connectionString))
            {
                RetryAttempts = retryAttempts
            };
            return client.Enqueue(job);
        }
    }
}