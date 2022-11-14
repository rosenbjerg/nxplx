using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models.File;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class FilmFileEntityTypeConfiguration : IEntityTypeConfiguration<FilmFile>
{
    public void Configure(EntityTypeBuilder<FilmFile> builder)
    {
        builder.HasIndex(ff => ff.PartOfLibraryId);
        builder.OwnsOne(ff => ff.MediaDetails);
        builder.HasOne(ff => ff.PartOfLibrary).WithMany().HasForeignKey(ff => ff.PartOfLibraryId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ff => ff.FilmDetails).WithMany().HasForeignKey(ff => ff.FilmDetailsId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ff => ff.FilmDetails).WithMany().HasForeignKey(ff => ff.FilmDetailsId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(ff => ff.Subtitles).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}