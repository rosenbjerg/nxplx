using System;
using System.Threading.Tasks;

namespace NxPlx.Domain.Services.Commands
{
    public abstract class CommandBase
    {
        public virtual string[] Arguments { get; } = Array.Empty<string>();
        public abstract Task<string> Execute(string[] arguments);
    }
}