using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NxPlx.Domain.Models;

namespace NxPlx.Infrastructure.Database.EntityTypeConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany<SubtitlePreference>().WithOne().HasForeignKey(sp => sp.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany<WatchingProgress>().WithOne().HasForeignKey(wp => wp.UserId).OnDelete(DeleteBehavior.Cascade);
        builder
            .Property(sl => sl.LibraryAccessIds)
            .HasConversion(
                ls => string.Join(',', ls),
                str => str.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
    }
}