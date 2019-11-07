using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NxPlx.Configuration;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Services.Database
{
    public class UserContext : DbContext
    {
        public DbSet<SubtitlePreference> SubtitlePreferences { get; set; }
        public DbSet<WatchingProgress> WatchingProgresses { get; set; }
        
        public DbSet<User> Users { get; set; }
        
        public DbSet<UserSession> UserSessions { get; set; }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         
            modelBuilder.Entity<SubtitlePreference>().HasKey(sp => new { sp.UserId, sp.FileId });
            modelBuilder.Entity<WatchingProgress>().HasKey(wp => new { wp.UserId, wp.FileId });
            
            modelBuilder.Entity<User>()
                .Property(sl => sl.LibraryAccessIds)
                .HasConversion(
                    ls => string.Join(',', ls),
                    str => str.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cfg = ConfigurationService.Current;
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseNpgsql($"Host={cfg.SqlHost};Database={cfg.SqlUserDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}");
        }
    }
}