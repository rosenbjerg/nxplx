using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Services.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly OperationContext _operationContext;
        public DatabaseContext(DbContextOptions<DatabaseContext> options, OperationContext operationContext)
            : base(options)
        {
            _operationContext = operationContext;
        }

        public DbSet<FilmFile> FilmFiles { get; set; } = null!;
        public DbSet<DbFilmDetails> FilmDetails { get; set; } = null!;
        public DbSet<EpisodeFile> EpisodeFiles { get; set; } = null!;
        public DbSet<DbSeriesDetails> SeriesDetails { get; set; } = null!;
        public DbSet<Library> Libraries { get; set; } = null!;
        public DbSet<SubtitleFile> SubtitleFiles { get; set; } = null!;
        
        public DbSet<SubtitlePreference> SubtitlePreferences { get; set; } = null!;
        public DbSet<WatchingProgress> WatchingProgresses { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;
        public DbSet<Genre> Genre { get; set; } = null!;
        public DbSet<MovieCollection> MovieCollection { get; set; } = null!;
        public DbSet<Network> Network { get; set; } = null!;
        public DbSet<ProductionCompany> ProductionCompany { get; set; } = null!;
        public DbSet<EpisodeDetails> EpisodeDetails { get; set; } = null!;
        public DbSet<SeasonDetails> SeasonDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            ConfigureEpisodeFileEntity(modelBuilder);

            ConfigureFilmFileEntity(modelBuilder);


            modelBuilder.Entity<DbSeriesDetails>().HasMany(s => s.Seasons).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SeasonDetails>().HasMany(s => s.Episodes).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DbFilmDetails>().HasOne(fd => fd.BelongsInCollection).WithMany(mc => mc.Movies).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductionCountry>().HasKey(pc => pc.Iso3166_1);
            modelBuilder.Entity<SpokenLanguage>().HasKey(sl => sl.Iso639_1);
            
            ConfigureFilmDetailsJoinEntities(modelBuilder);

            ConfigureSeriesDetailsJoinEntities(modelBuilder);
            
            modelBuilder.Entity<SubtitlePreference>().HasKey(sp => new { sp.UserId, sp.FileId });
            modelBuilder.Entity<SubtitlePreference>().HasIndex(sp => sp.UserId);
            modelBuilder.Entity<SubtitlePreference>().HasIndex(sp => sp.FileId);
            
            modelBuilder.Entity<WatchingProgress>().HasKey(wp => new { wp.UserId, wp.FileId });
            modelBuilder.Entity<WatchingProgress>().HasIndex(wp => wp.UserId);
            modelBuilder.Entity<WatchingProgress>().HasIndex(wp => wp.FileId);
            
            modelBuilder.Entity<User>().HasMany<SubtitlePreference>().WithOne().HasForeignKey(sp => sp.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasMany<WatchingProgress>().WithOne().HasForeignKey(wp => wp.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserSession>().HasOne(us => us.User).WithMany().OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<User>()
                .Property(sl => sl.LibraryAccessIds)
                .HasConversion(
                    ls => string.Join(',', ls),
                    str => str.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

            if (_operationContext?.User != null && !_operationContext.User.Admin)
            {
                modelBuilder.Entity<User>().HasQueryFilter(e => e.Id == _operationContext.User.Id);
                modelBuilder.Entity<UserSession>().HasQueryFilter(e => e.UserId == _operationContext.User.Id);
                modelBuilder.Entity<SubtitlePreference>().HasQueryFilter(e => e.UserId == _operationContext.User.Id);
                modelBuilder.Entity<WatchingProgress>().HasQueryFilter(e => e.UserId == _operationContext.User.Id);
                
                modelBuilder.Entity<Library>().HasQueryFilter(e => _operationContext.User.LibraryAccessIds.Contains(e.Id));
                modelBuilder.Entity<FilmFile>().HasQueryFilter(e => _operationContext.User.LibraryAccessIds.Contains(e.PartOfLibraryId));
                modelBuilder.Entity<EpisodeFile>().HasQueryFilter(e => _operationContext.User.LibraryAccessIds.Contains(e.PartOfLibraryId));
            }
        }

        private static void ConfigureSeriesDetailsJoinEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Genre>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Genre>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Genre>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JoinEntity<DbSeriesDetails, ProductionCompany>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, ProductionCompany>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, ProductionCompany>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Creator>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Creator>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Creator>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Network>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Network>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbSeriesDetails, Network>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureFilmDetailsJoinEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JoinEntity<DbFilmDetails, Genre>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, Genre>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbFilmDetails, Genre>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCompany>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCompany>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCompany>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCountry, string>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCountry, string>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbFilmDetails, ProductionCountry, string>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<JoinEntity<DbFilmDetails, SpokenLanguage, string>>().HasKey(e => new { e.Entity1Id, e.Entity2Id });
            modelBuilder.Entity<JoinEntity<DbFilmDetails, SpokenLanguage, string>>().HasOne(o => o.Entity1).WithMany().HasForeignKey(o => o.Entity1Id).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JoinEntity<DbFilmDetails, SpokenLanguage, string>>().HasOne(o => o.Entity2).WithMany().HasForeignKey(o => o.Entity2Id).OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureFilmFileEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilmFile>().HasIndex(ff => ff.PartOfLibraryId);
            modelBuilder.Entity<FilmFile>().HasOne(ff => ff.FilmDetails).WithMany().HasForeignKey(ff => ff.FilmDetailsId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FilmFile>().HasOne(ff => ff.PartOfLibrary).WithMany().HasForeignKey(ff => ff.PartOfLibraryId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FilmFile>().HasMany(ff => ff.Subtitles).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FilmFile>().OwnsOne(ff => ff.MediaDetails);
        }

        private static void ConfigureEpisodeFileEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpisodeFile>().HasIndex(ef => ef.SeasonNumber);
            modelBuilder.Entity<EpisodeFile>().HasIndex(ef => ef.PartOfLibraryId);
            modelBuilder.Entity<EpisodeFile>().HasOne(ef => ef.SeriesDetails).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EpisodeFile>().Ignore(ef => ef.SeasonDetails).Ignore(ef => ef.EpisodeDetails);
            modelBuilder.Entity<EpisodeFile>().HasOne(ef => ef.PartOfLibrary).WithMany().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EpisodeFile>().HasMany(ef => ef.Subtitles).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EpisodeFile>().OwnsOne(ef => ef.MediaDetails);
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