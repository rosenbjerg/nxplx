using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class SessionService
    {
        private readonly OperationContext _operationContext;
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;

        public SessionService(OperationContext operationContext, DatabaseContext context, IDtoMapper dtoMapper)
        {
            _operationContext = operationContext;
            _context = context;
            _dtoMapper = dtoMapper;
        }
        public async Task<bool> CloseUserSession(string sessionId)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(us => us.Id == sessionId);
            if (session == default || session.UserId != _operationContext.User.Id && !session.User.Admin)
                return false;

            _context.UserSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<UserSessionDto>> GetUserSessions(int userId)
        {
            var sessions = await _context.UserSessions.Where(s => s.UserId == userId).ToListAsync();
            return _dtoMapper.Map<UserSession, UserSessionDto>(sessions);
        }
    }
}