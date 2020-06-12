using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services.Commands
{
    public class DeleteSubtitlePreferencesCommand : CommandBase
    {
        private readonly DatabaseContext _context;

        public DeleteSubtitlePreferencesCommand(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<string> Execute(string[] args)
        {
            var userIds = await _context.Users.Select(u => u.Id).ToListAsync();
            var deletedCount = 0;

            foreach (var userId in userIds)
            {
                var prefs = await _context.SubtitlePreferences.Where(sp => sp.UserId == userId).ToListAsync();
                _context.SubtitlePreferences.RemoveRange(prefs);
                await _context.SaveChangesAsync();
                deletedCount += prefs.Count;
            }

            return $"Removed {deletedCount} subtitle preferences for {userIds.Count} users";
        }
    }
}