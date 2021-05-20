using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Domain.Events;

namespace NxPlx.Domain.Services.EventHandlers.AdminCommand
{
    public class AdminCommandInvocationCommandHandler : AdminCommandHandler<AdminCommandInvocationCommand, string?>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AdminCommandInvocationCommandHandler> _logger;

        public AdminCommandInvocationCommandHandler(IServiceProvider serviceProvider, ILogger<AdminCommandInvocationCommandHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public override async Task<string?> Handle(AdminCommandInvocationCommand @event, CancellationToken cancellationToken = default)
        {
            if (!Commands.TryGetValue(@event.CommandName, out var commandResolver)) return null;
            var cmd = commandResolver(_serviceProvider);
            _logger.LogInformation("Invoking {AdminCommandName} with arguments {Arguments}", @event.CommandName, @event.Arguments);
            return await cmd.Execute(@event.Arguments);
        }
    }
}