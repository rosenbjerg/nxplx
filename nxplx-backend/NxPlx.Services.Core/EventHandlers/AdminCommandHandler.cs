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
    public class AdminCommandHandler : IEventHandler<AdminCommandInvocationCommand, string?>, IEventHandler<AdminCommandListRequestQuery, string[]>
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

        public async Task<string?> Handle(AdminCommandInvocationCommand command, CancellationToken cancellationToken = default)
        {
            if (!Commands.TryGetValue(command.CommandName, out var commandResolver)) return null;
            var cmd = commandResolver(_serviceProvider);
            _logger.LogInformation("Invoking {AdminCommandName} with arguments {Arguments}", command.CommandName, command.Arguments);
            return await cmd.Execute(command.Arguments);
        }

        public Task<string[]> Handle(AdminCommandListRequestQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Commands.Keys.ToArray());
        }

        private static Func<IServiceProvider, CommandBase> GetResolver(Type type) => provider => (CommandBase) provider.GetService(type);
    }
}