using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models.Details.Film;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class ProductionCountryEntityTypeConfiguration : IEntityTypeConfiguration<ProductionCountry>
{
    public void Configure(EntityTypeBuilder<ProductionCountry> builder)
    {
        builder.HasKey(pc => pc.Iso3166_1);
    }
}