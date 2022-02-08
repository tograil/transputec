using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class StdTimeZoneConfiguration : IEntityTypeConfiguration<StdTimeZone>
{
    public void Configure(EntityTypeBuilder<StdTimeZone> builder)
    {
        builder.HasKey(e => e.TimeZoneId)
            .HasName("PK_dbo.StdTimeZone");

        builder.ToTable("StdTimeZone");

        builder.Property(e => e.TimeZoneId).HasColumnName("TimeZoneID");

        builder.Property(e => e.PortalTimeZone).HasMaxLength(100);

        builder.Property(e => e.ZoneId).HasMaxLength(100);

        builder.Property(e => e.ZoneLabel).HasMaxLength(100);
    }
}