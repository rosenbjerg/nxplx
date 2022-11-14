using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class SubtitlePreferenceEntityTypeConfiguration : IEntityTypeConfiguration<SubtitlePreference>
{
    public void Configure(EntityTypeBuilder<SubtitlePreference> builder)
    {
        builder.HasKey(sp => new { sp.UserId, sp.FileId, sp.MediaType });
        builder.HasIndex(sp => sp.UserId);
        builder.HasIndex(sp => sp.FileId);
        builder.HasIndex(sp => sp.MediaType);
    }
}