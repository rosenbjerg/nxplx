using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Models.Events;
using NxPlx.Core.Services.Commands;

namespace NxPlx.Core.Services.EventHandlers
{
    public class AdminCommandHandler : IEventHandler<AdminCommandInvocationEvent, string?>, IEventHandler<AdminCommandListRequestEvent, string[]>
    {
        private static readonly Dictionary<string, Func<IServiceProvider, CommandBase>> Commands = new List<Type>
        {
            typeof(DeleteWatchingProgressCommand),
            typeof(DeleteSubtitlePreferencesCommand)
        }.ToDictionary(type => type.Name, GetResolver);
        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AdminCommandHandler> _logger;

        public AdminCommandHandler(IServiceProvider serviceProvider, ILogger<AdminCommandHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<string?> Handle(AdminCommandInvocationEvent @event, CancellationToken cancellationToken = default)
        {
            if (!Commands.TryGetValue(@event.CommandName, out var commandResolver)) return null;
            var command = commandResolver(_serviceProvider);
            _logger.LogInformation("Invoking {AdminCommandName} with arguments {Arguments}", @event.CommandName, @event.Arguments);
            return await command.Execute(@event.Arguments);
        }

        public Task<string[]> Handle(AdminCommandListRequestEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Commands.Keys.ToArray());
        }

        private static Func<IServiceProvider, CommandBase> GetResolver(Type type) => provider => (CommandBase) provider.GetService(type);
    }
}