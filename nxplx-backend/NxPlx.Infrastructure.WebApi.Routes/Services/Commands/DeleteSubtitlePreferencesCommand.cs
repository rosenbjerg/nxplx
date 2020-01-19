using System.Threading.Tasks;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Services.Database.Wrapper;

namespace NxPlx.Infrastructure.WebApi.Routes.Services.Commands
{
    public class DeleteSubtitlePreferencesCommand : CommandBase
    {
        public override string Description => "Deletes all subtitle preferences in the database";

        public override async Task<string> Execute(string[] args)
        {
            await using var ctx = ResolveContainer.Default.Resolve<ReadNxplxContext>();
            var transaction = ctx.BeginTransactionedContext();
            await foreach (var batch in ctx.SubtitlePreferences.Many().Batched(500))
            {
                transaction.SubtitlePreferences.Remove(batch);
            }

            return "All subtitle preferences has been removed";
        }
    }
}