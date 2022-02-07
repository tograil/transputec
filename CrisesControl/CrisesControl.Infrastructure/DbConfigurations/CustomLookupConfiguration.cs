using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CustomLookupConfiguration : IEntityTypeConfiguration<CustomLookup>
{
    public void Configure(EntityTypeBuilder<CustomLookup> builder)
    {
        builder.HasKey(e => e.LookupId);

        builder.ToTable("CustomLookup");

        builder.Property(e => e.LookupId).HasColumnName("LookupID");

        builder.Property(e => e.LookupCategory)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.LookupLabel).HasMaxLength(50);

        builder.Property(e => e.LookupValue)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}