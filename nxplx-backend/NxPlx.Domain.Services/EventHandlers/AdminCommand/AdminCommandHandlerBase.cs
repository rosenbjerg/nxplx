using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NxPlx.Domain.Services.Commands;
using NxPlx.Infrastructure.Events.Events;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.AdminCommand
{
    public abstract class AdminCommandHandler<TEvent, TResult> : IDomainEventHandler<TEvent, TResult>
        where TEvent : IDomainEvent<TResult>
    {
        protected static readonly Dictionary<string, Func<IServiceProvider, CommandBase>> Commands = new List<Type>
        {
            typeof(DeleteWatchingProgressCommand),
            typeof(DeleteSubtitlePreferencesCommand)
        }.ToDictionary(type => type.Name, GetResolver);

        private static Func<IServiceProvider, CommandBase> GetResolver(Type type) => provider => (CommandBase) provider.GetService(type)!;
        public abstract Task<TResult> Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
}