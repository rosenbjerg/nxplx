using System.Threading.Tasks;

namespace NxPlx.Domain.Services.Commands
{
    public abstract class CommandBase
    {
        public virtual string[] Arguments { get; } = new string[0];
        public abstract Task<string> Execute(string[] arguments);
    }
}