using Microsoft.EntityFrameworkCore;
using NxPlx.Configuration;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;
using NxPlx.Services.Database.Models;

namespace NxPlx.Services.Database
{
    public class MediaContext : DbContext
    {
        public DbSet<FilmFile> FilmFiles { get; set; }
        public DbSet<EpisodeFile> EpisodeFiles { get; set; }
        
        public DbSet<DbFilmDetails> FilmDetails { get; set; }
        public DbSet<DbSeriesDetails> SeriesDetails { get; set; }
        public DbSet<SeasonDetails> SeasonsDetails { get; set; }
        public DbSet<EpisodeDetails> EpisodeDetails { get; set; }
        
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ProductionCompany> ProductionCompanies { get; set; }
        public DbSet<ProductionCountry> ProductionCountries { get; set; }
        public DbSet<SpokenLanguage> SpokenLanguages { get; set; }
        public DbSet<MovieCollection> MovieCollections { get; set; }
        public DbSet<Creator> Creators { get; set; }
        public DbSet<Network> Networks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
//            modelBuilder.Entity<EpisodeFile>().HasOne(sf => sf.SeriesDetails).WithMany();
            modelBuilder.Entity<EpisodeFile>().HasIndex(ef => ef.SeasonNumber);

//            modelBuilder.Entity<FilmFile>().HasOne(ff => ff.FilmDetails).WithMany();
            
            modelBuilder.Entity<ProductionCountry>().HasKey(pc => pc.Iso3166_1);
            modelBuilder.Entity<SpokenLanguage>().HasKey(sl => sl.Iso639_1);



            modelBuilder.Entity<DbSeriesDetails>().HasMany(s => s.Seasons);
            modelBuilder.Entity<SeasonDetails>().HasMany(s => s.Episodes);
            
            
            modelBuilder.Entity<JoinEntity<DbFilmDetails, Genre>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCompany>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCountry, string>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, SpokenLanguage, string>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<DbFilmDetails>().HasOne(fd => fd.BelongsInCollection).WithMany();
            
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Genre>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, ProductionCompany>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Creator>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Network>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cfg = ConfigurationService.Current;
            optionsBuilder.UseNpgsql($"Host={cfg.SqlHost};Database={cfg.SqlMediaDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}");
        }
        
    }
}