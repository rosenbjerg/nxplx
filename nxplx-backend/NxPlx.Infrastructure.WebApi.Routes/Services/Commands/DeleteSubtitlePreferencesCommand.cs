using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services.Commands
{
    public class DeleteSubtitlePreferencesCommand : CommandBase
    {
        public override string Description => "Deletes all subtitle preferences in the database";

        public override async Task<string> Execute(string[] args)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>();
            var transaction = ctx.BeginTransactionedContext();

            var userIds = await ctx.Users.ProjectMany(null, u => u.Id).ToListAsync();

            foreach (var userId in userIds)
            {
                var prefs = await ctx.SubtitlePreferences.Many(sp => sp.UserId == userId).ToListAsync();
                transaction.SubtitlePreferences.Remove(prefs);
            }

            await transaction.SaveChanges();
            return "All subtitle preferences has been removed";
        }
    }
}