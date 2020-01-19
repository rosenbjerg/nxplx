using System.Threading.Tasks;

namespace NxPlx.Models
{
    public abstract class CommandBase
    {
        public string Name => GetType().Name;
        public virtual string[] Arguments { get; } = new string[0];
        public virtual string Description { get; } = "No description";
        public abstract Task<string> Execute(string[] arguments);
    }
}