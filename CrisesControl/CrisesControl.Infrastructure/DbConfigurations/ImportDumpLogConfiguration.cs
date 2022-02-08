using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ImportDumpLogConfiguration : IEntityTypeConfiguration<ImportDumpLog>
{
    public void Configure(EntityTypeBuilder<ImportDumpLog> builder)
    {
        builder.HasKey(e => e.ImportDumpHistoryId)
            .HasName("PK_dbo.ImportDumpLog");

        builder.ToTable("ImportDumpLog");

        builder.Property(e => e.Isd).HasColumnName("ISD");

        builder.Property(e => e.Llisd).HasColumnName("LLISD");

        builder.Property(e => e.LocLat)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.LocLng)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.LogDate).HasDefaultValueSql("(sysdatetimeoffset())");
    }
}