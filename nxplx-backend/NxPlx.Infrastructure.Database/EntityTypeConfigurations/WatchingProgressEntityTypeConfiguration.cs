using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class WatchingProgressEntityTypeConfiguration : IEntityTypeConfiguration<WatchingProgress>
{
    public void Configure(EntityTypeBuilder<WatchingProgress> builder)
    {
        builder.HasKey(wp => new { wp.UserId, wp.FileId, wp.MediaType });
        builder.HasIndex(wp => wp.UserId);
        builder.HasIndex(wp => wp.FileId);
        builder.HasIndex(wp => wp.MediaType);
    }
}