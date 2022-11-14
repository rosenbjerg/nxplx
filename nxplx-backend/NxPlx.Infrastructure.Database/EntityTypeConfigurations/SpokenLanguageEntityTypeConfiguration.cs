using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models.Details.Film;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class SpokenLanguageEntityTypeConfiguration : IEntityTypeConfiguration<SpokenLanguage>
{
    public void Configure(EntityTypeBuilder<SpokenLanguage> builder)
    {
        builder.HasKey(sl => sl.Iso639_1);
    }
}