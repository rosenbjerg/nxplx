using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Configuration;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Services.Database
{
    public class MediaContext : DbContext
    {
        public DbSet<FilmFile> FilmFiles { get; set; }
        public DbSet<EpisodeFile> EpisodeFiles { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<SubtitleFile> SubtitleFiles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<EpisodeFile>()
                .HasOne(sf => sf.SeriesDetails)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EpisodeFile>()
                .HasOne(sf => sf.PartOfLibrary)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<FilmFile>()
                .HasOne(ff => ff.FilmDetails)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);  
            modelBuilder.Entity<FilmFile>()
                .HasOne(ff => ff.PartOfLibrary)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<EpisodeFile>().HasIndex(ef => ef.SeasonNumber);
            
            modelBuilder.Entity<DbSeriesDetails>().HasMany(s => s.Seasons).WithOne();
            modelBuilder.Entity<SeasonDetails>().HasMany(s => s.Episodes).WithOne();
            modelBuilder.Entity<DbFilmDetails>().HasOne(fd => fd.BelongsInCollection).WithMany();
            
            modelBuilder.Entity<ProductionCountry>().HasKey(pc => pc.Iso3166_1);
            modelBuilder.Entity<SpokenLanguage>().HasKey(sl => sl.Iso639_1);
            
            modelBuilder.Entity<JoinEntity<DbFilmDetails, Genre>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCompany>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCountry, string>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, SpokenLanguage, string>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Genre>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, ProductionCompany>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Creator>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Network>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cfg = ConfigurationService.Current;
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseNpgsql($"Host={cfg.SqlHost};Database={cfg.SqlMediaDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}");
            
        }
    }

    public static class DbUtils
    {
        public static async Task AddOrUpdate<TEntity>(this DbContext context, IList<TEntity> entities)
            where TEntity : EntityBase
        {
            var set = context.Set<TEntity>();
            var ids = entities.Where(e => e.Id != default).Select(e => e.Id).ToList();
            var existing = await set.Where(e => ids.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
            foreach (var entity in entities)
            {
                if (existing.TryGetValue(entity.Id, out var existingEntity))
                    context.Entry(existingEntity).CurrentValues.SetValues(entity);
                else
                    set.Add(entity);
            }
        }
    }
}