using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Domain.Models;
using NxPlx.Domain.Models.Database;
using NxPlx.Domain.Models.Details;
using NxPlx.Domain.Models.Details.Film;
using NxPlx.Domain.Models.Details.Series;
using NxPlx.Domain.Models.File;

namespace NxPlx.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly IOperationContext _operationContext;
        public DatabaseContext(DbContextOptions<DatabaseContext> options, IOperationContext operationContext)
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
        public DbSet<Genre> Genre { get; set; } = null!;
        public DbSet<MovieCollection> MovieCollection { get; set; } = null!;
        public DbSet<Network> Network { get; set; } = null!;
        public DbSet<ProductionCompany> ProductionCompany { get; set; } = null!;
        public DbSet<EpisodeDetails> EpisodeDetails { get; set; } = null!;
        public DbSet<SeasonDetails> SeasonDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
            
            modelBuilder.Entity<User>().HasQueryFilter(e => _operationContext.Session == null || (_operationContext.Session.IsAdmin || e.Id == _operationContext.Session.UserId));
            modelBuilder.Entity<SubtitlePreference>().HasQueryFilter(e => _operationContext.Session != null && (_operationContext.Session.IsAdmin || e.UserId == _operationContext.Session.UserId));
            modelBuilder.Entity<WatchingProgress>().HasQueryFilter(e => _operationContext.Session != null && (_operationContext.Session.IsAdmin || e.UserId == _operationContext.Session.UserId));
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is ITrackedEntity && e.State is EntityState.Added or EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                var entity = (ITrackedEntity)entityEntry.Entity;
                
                if (entityEntry.State == EntityState.Added)
                {
                    entity.Created = DateTime.UtcNow;
                    entity.CreatedCorrelationId = _operationContext.CorrelationId;
                }
                
                entity.Updated = DateTime.UtcNow;
                entity.UpdatedCorrelationId = _operationContext.CorrelationId;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}