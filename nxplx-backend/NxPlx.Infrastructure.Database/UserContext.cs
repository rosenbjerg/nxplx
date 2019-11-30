using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NxPlx.Configuration;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;

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