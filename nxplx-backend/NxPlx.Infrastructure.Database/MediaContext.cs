using System.IO;
using Microsoft.EntityFrameworkCore;
using NxPlx.Configuration;
using NxPlx.Models;
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
        
        public DbSet<FilmDetails> Film { get; set; }
        
        public DbSet<SeriesDetails> Series { get; set; }
        
        public DbSet<Genre> Genres { get; set; }
        
        public DbSet<MovieCollection> MovieCollections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<EpisodeFile>().HasIndex(ef => ef.SeriesDetailsId);
            modelBuilder.Entity<EpisodeFile>().HasIndex(ef => ef.SeasonNumber);
            
            modelBuilder.Entity<FilmFile>().HasIndex(ff => ff.FilmDetailsId);
            
            modelBuilder.Entity<ProductionCountry>().HasKey(pc => pc.Iso3166_1);
            modelBuilder.Entity<SpokenLanguage>().HasKey(sl => sl.Iso6391);
            
            modelBuilder.Entity<InGenre>().HasKey(e =>    new { e.DetailsEntityId, e.GenreId });
            modelBuilder.Entity<ProducedBy>().HasKey(e => new { e.DetailsEntityId, e.ProductionCompanyId });
            modelBuilder.Entity<Season>().HasKey(e => new { e.SeriesDetailsId, e.SeasonDetailsId });
            
            modelBuilder.Entity<CreatedBy>().HasKey(e =>   new { e.SeriesDetailsId, e.CreatorId });
            modelBuilder.Entity<BroadcastOn>().HasKey(e => new { e.SeriesDetailsId, e.NetworkId });
            
            modelBuilder.Entity<ProducedIn>().HasKey(e =>          new { e.FilmDetailsId, e.ProductionCountryId });
            modelBuilder.Entity<LanguageSpoken>().HasKey(e =>      new { e.FilmDetailsId, e.SpokenLanguageId });
            modelBuilder.Entity<BelongsInCollection>().HasKey(e => new { e.FilmDetailsId, e.MovieCollectionId });
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cfg = ConfigurationService.Current;
            optionsBuilder.UseNpgsql($"Host={cfg.SqlHost};Database={cfg.SqlMediaDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}");
        }
        
    }
}