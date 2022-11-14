using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models.File;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class EpisodeFileEntityTypeConfiguration : IEntityTypeConfiguration<EpisodeFile>
{
    public void Configure(EntityTypeBuilder<EpisodeFile> builder)
    {
        builder.HasIndex(ef => ef.SeasonNumber);
        builder.HasIndex(ef => ef.PartOfLibraryId);
        builder.HasOne(ef => ef.SeriesDetails).WithMany().OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(ef => ef.SeasonDetails).Ignore(ef => ef.EpisodeDetails);
        builder.HasOne(ef => ef.PartOfLibrary).WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(ef => ef.Subtitles).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.OwnsOne(ef => ef.MediaDetails);
    }
}