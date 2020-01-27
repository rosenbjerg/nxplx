using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services.Commands
{
    public class DeleteWatchingProgressCommand : CommandBase
    {
        public override string Description => "Deletes all watching progress in the database";
        public override async Task<string> Execute(string[] args)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>();
            var transaction = ctx.BeginTransactionedContext();

            var userIds = await ctx.Users.ProjectMany(null, u => u.Id).ToListAsync();

            foreach (var userId in userIds)
            {
                var progress = await ctx.WatchingProgresses.Many(wp => wp.UserId == userId).ToListAsync();
                transaction.WatchingProgresses.Remove(progress);
            }

            await transaction.SaveChanges();
            return "All watching progress has been removed";
        }
    }
}