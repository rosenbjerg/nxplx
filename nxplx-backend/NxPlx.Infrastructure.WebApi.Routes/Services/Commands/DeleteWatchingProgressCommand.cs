using System.Threading.Tasks;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Services.Database.Wrapper;

namespace NxPlx.Infrastructure.WebApi.Routes.Services.Commands
{
    public class DeleteWatchingProgressCommand : CommandBase
    {
        public override string Description => "Deletes all watching progress in the database";
        public override async Task<string> Execute(string[] args)
        {
            await using var ctx = ResolveContainer.Default.Resolve<ReadNxplxContext>();
            var transaction = ctx.BeginTransactionedContext();
            await foreach (var batch in ctx.WatchingProgresses.Many().Batched(500))
            {
                transaction.WatchingProgresses.Remove(batch);
            }
            
            return "All watching progress has been removed";
        }
    }
}