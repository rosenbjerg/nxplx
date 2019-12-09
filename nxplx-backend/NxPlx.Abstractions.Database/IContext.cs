using System;
using System.Threading.Tasks;

namespace NxPlx.Abstractions.Database
{
    public interface IContext : IAsyncDisposable
    {
        Task SaveChanges();
    }
}