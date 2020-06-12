using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Core.Services.Commands;
using NxPlx.Models;

namespace NxPlx.Core.Services
{
    public class AdminCommandService
    {
        private readonly IServiceProvider _serviceProvider;

        public AdminCommandService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static readonly Dictionary<string, Func<IServiceProvider, CommandBase>> Commands = new List<Type>
        {
            typeof(DeleteWatchingProgressCommand),
            typeof(DeleteSubtitlePreferencesCommand)
        }.ToDictionary(type => type.Name, GetResolver);

        private static Func<IServiceProvider, CommandBase> GetResolver(Type type) => provider => (CommandBase) provider.GetService(type);

        public async Task<string?> InvokeCommand(string commandName, string[] args)
        {
            if (!Commands.TryGetValue(commandName, out var commandResolver)) return null;
            var command = commandResolver(_serviceProvider);
            return await command.Execute(args);

        }

        public IEnumerable<string> AvailableCommands() => Commands.Keys;
    }
}