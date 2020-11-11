using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class CurrentUserLookupEventHandler : IEventHandler<CurrentUserLookupEvent, UserDto>
    {
        private readonly IDtoMapper _dtoMapper;
        private readonly UserContextService _userContextService;

        public CurrentUserLookupEventHandler(IDtoMapper dtoMapper, UserContextService userContextService)
        {
            _dtoMapper = dtoMapper;
            _userContextService = userContextService;
        }
        
        public async Task<UserDto> Handle(CurrentUserLookupEvent @event, CancellationToken cancellationToken = default)
        {
            return _dtoMapper.Map<Models.User, UserDto>(await _userContextService.GetUser())!;
        }
    }
}