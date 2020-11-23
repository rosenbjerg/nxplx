using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Events.User;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class CurrentUserLookupQueryHandler : IEventHandler<CurrentUserLookupQuery, UserDto>
    {
        private readonly IDtoMapper _dtoMapper;
        private readonly IEventDispatcher _dispatcher;

        public CurrentUserLookupQueryHandler(IDtoMapper dtoMapper, IEventDispatcher dispatcher)
        {
            _dtoMapper = dtoMapper;
            _dispatcher = dispatcher;
        }
        
        public async Task<UserDto> Handle(CurrentUserLookupQuery query, CancellationToken cancellationToken = default)
        {
            var currentUser = await _dispatcher.Dispatch(new CurrentUserQuery());
            return _dtoMapper.Map<Models.User, UserDto>(currentUser)!;
        }
    }
}