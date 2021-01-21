using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class SubtitleLanguagePreferenceQueryHandler : IEventHandler<SubtitleLanguagePreferenceQuery, string>
    {
        private readonly DatabaseContext _context;
        private readonly IOperationContext _operationContext;

        public SubtitleLanguagePreferenceQueryHandler(DatabaseContext context, IOperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }
        public async Task<string> Handle(SubtitleLanguagePreferenceQuery @event, CancellationToken cancellationToken = default)
        {
            var preference = await _context.SubtitlePreferences.AsNoTracking()
                .Where(sp => sp.UserId == _operationContext.Session.UserId && sp.FileId == @event.FileId && sp.MediaType == @event.MediaType)
                .Select(sp => sp.Language)
                .FirstOrDefaultAsync(cancellationToken);
            return preference ?? "none";
        }
    }
}