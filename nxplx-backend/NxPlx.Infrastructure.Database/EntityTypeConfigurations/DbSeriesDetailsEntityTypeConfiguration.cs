using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models.Database;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class DbSeriesDetailsEntityTypeConfiguration : IEntityTypeConfiguration<DbSeriesDetails>
{
    public void Configure(EntityTypeBuilder<DbSeriesDetails> builder)
    {
        builder.HasMany(s => s.Seasons).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.ProductionCompanies).WithMany(pc => pc.Series);
        builder.HasMany(s => s.Genres).WithMany(g => g.Series);
        builder.HasMany(s => s.Networks).WithMany(g => g.Series);
        builder.HasMany(s => s.CreatedBy).WithMany(g => g.Series);
    }
}