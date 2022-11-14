using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models.Database;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class DbFilmDetailsEntityTypeConfiguration : IEntityTypeConfiguration<DbFilmDetails>
{
    public void Configure(EntityTypeBuilder<DbFilmDetails> builder)
    {
        builder.HasOne(fd => fd.BelongsInCollection).WithMany(mc => mc.Movies).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(fd => fd.Genres).WithMany(g => g.Film);
        builder.HasMany(fd => fd.ProductionCompanies).WithMany(g => g.Film);
        builder.HasMany(fd => fd.ProductionCountries).WithMany(g => g.Film);
        builder.HasMany(fd => fd.SpokenLanguages).WithMany(g => g.Film);
    }
}