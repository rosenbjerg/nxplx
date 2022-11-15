using Microsoft.EntityFrameworkCore;

namespace NxPlx.Infrastructure.Database
{
    public class HangfireContext : DbContext
    {
        public HangfireContext(DbContextOptions<HangfireContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.HasDefaultSchema("hangfire");
        }
    }
}