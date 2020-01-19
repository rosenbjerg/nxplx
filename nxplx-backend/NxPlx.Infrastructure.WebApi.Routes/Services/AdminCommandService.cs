using System;
using System.Threading.Tasks;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public class AdminCommandService
    {
        
    }

    public abstract class CommandBase
    {
        public abstract string[] Arguments { get; }
        public abstract Task Execute();
    }
    
    public class DeleteWatchingProgressCommand : CommandBase 
    {
        public override string[] Arguments { get; }
        public override Task Execute()
        {
            throw new NotImplementedException();
        }
    }
}