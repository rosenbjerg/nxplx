using Microsoft.EntityFrameworkCore;

namespace NxPlx.Services.Database
{
    public class HangfireContext : DbContext
    {
        private readonly string _connectionString;

        public static void EnsureCreated(string connectionString)
        {
            using var context = new HangfireContext(connectionString);
            context.Database.EnsureCreated();
        }

        private HangfireContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseNpgsql(_connectionString));
        }
    }
}