using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Domain.Events.Series;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Series
{
    public class SetSubtitleLanguagePreferenceCommandHandler : IDomainEventHandler<SetSubtitleLanguagePreferenceCommand>
    {
        private readonly DatabaseContext _context;
        private readonly IOperationContext _operationContext;

        public SetSubtitleLanguagePreferenceCommandHandler(DatabaseContext context, IOperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }
        
        public async Task Handle(SetSubtitleLanguagePreferenceCommand @event, CancellationToken cancellationToken = default)
        {
            var preference = await _context.SubtitlePreferences.FirstOrDefaultAsync(sp => sp.UserId == _operationContext.Session.UserId && sp.FileId == @event.FileId && sp.MediaType == @event.MediaType, CancellationToken.None);
            if (preference == null)
            {
                preference = new SubtitlePreference { UserId = _operationContext.Session.UserId, FileId = @event.FileId, MediaType = @event.MediaType};
                _context.SubtitlePreferences.Add(preference);
            }

            preference.Language = @event.Language;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}