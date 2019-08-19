using System.IO;
using Microsoft.EntityFrameworkCore;
using NxPlx.Configuration;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Services.Database
{
    public class MediaContext : DbContext
    {
        public DbSet<FilmFile> FilmFiles { get; set; }

        public DbSet<EpisodeFile> EpisodeFiles { get; set; }
        
        public DbSet<SubtitleFile> SubtitleFiles { get; set; }
        
        public DbSet<Film> Film { get; set; }
        
        public DbSet<Episode> Episodes { get; set; }
        
        public DbSet<Series> Series { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cfg = ConfigurationService.Current;
            optionsBuilder.UseNpgsql($"Host={cfg.SqlHost};Database={cfg.SqlMediaDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}");
        }
        
    }
}