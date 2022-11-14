using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models.Details.Series;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class SeasonDetailsEntityTypeConfiguration : IEntityTypeConfiguration<SeasonDetails>
{
    public void Configure(EntityTypeBuilder<SeasonDetails> builder)
    {
        builder.HasMany(s => s.Episodes).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}