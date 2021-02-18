using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using NxPlx.Application.Models;
using NxPlx.Domain.Events;
using NxPlx.Domain.Events.User;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.User
{
    public class CurrentUserLookupQueryHandler : IDomainEventHandler<CurrentUserLookupQuery, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IDomainEventDispatcher _dispatcher;

        public CurrentUserLookupQueryHandler(IMapper mapper, IDomainEventDispatcher dispatcher)
        {
            _mapper = mapper;
            _dispatcher = dispatcher;
        }
        
        public async Task<UserDto> Handle(CurrentUserLookupQuery query, CancellationToken cancellationToken = default)
        {
            var currentUser = await _dispatcher.Dispatch(new CurrentUserQuery());
            return _mapper.Map<Domain.Models.User, UserDto>(currentUser)!;
        }
    }
}