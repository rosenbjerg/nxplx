using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Domain.Services.Commands
{
    public class DeleteWatchingProgressCommand : CommandBase
    {
        private readonly DatabaseContext _context;

        public DeleteWatchingProgressCommand(DatabaseContext context)
        {
            _context = context;
        }
        
        public override async Task<string> Execute(string[] args)
        {
            var userIds = await _context.Users.Select(u => u.Id).ToListAsync();
            var deletedCount = 0;

            foreach (var userId in userIds)
            {
                var progress = await _context.WatchingProgresses.Where(wp => wp.UserId == userId).ToListAsync();
                _context.WatchingProgresses.RemoveRange(progress);
                await _context.SaveChangesAsync();
            }

            return $"Removed watching progress for {deletedCount} items across {userIds.Count} users";
        }
    }
}