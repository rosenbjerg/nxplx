using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;

namespace NxPlx.Infrastructure.Database
{
    public class ReadOnlyDatabaseContext : DatabaseContext
    {
        public ReadOnlyDatabaseContext(DbContextOptions<DatabaseContext> options, IOperationContext operationContext) : base(options, operationContext)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseChangeTrackingProxies(false, false);
            base.OnConfiguring(optionsBuilder);
        }
    }
}