using System;

namespace NxPlx.Abstractions.Database
{
    public interface IReadContext<out TWriteContext> : IAsyncDisposable
        where TWriteContext : IContext
    {
        TWriteContext BeginTransactionedContext();
    }
}