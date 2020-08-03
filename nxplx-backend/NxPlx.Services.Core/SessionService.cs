using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models;
using NxPlx.Services.Database;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Core.Services
{
    public class SessionService
    {
        private readonly OperationContext _operationContext;
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;
        private readonly IMapper _mapper;

        public SessionService(OperationContext operationContext, DatabaseContext context, IDtoMapper dtoMapper, IMapper mapper)
        {
            _operationContext = operationContext;
            _context = context;
            _dtoMapper = dtoMapper;
            _mapper = mapper;
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
        public Task<List<UserSessionDto>> GetUserSessions(int userId)
        {
            return _context.UserSessions
                .Where(s => s.UserId == userId)
                .ProjectTo<UserSessionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}