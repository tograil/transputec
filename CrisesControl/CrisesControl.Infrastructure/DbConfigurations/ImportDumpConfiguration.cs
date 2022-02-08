using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ImportDumpConfiguration : IEntityTypeConfiguration<ImportDump>
{
    public void Configure(EntityTypeBuilder<ImportDump> builder)
    {
        builder.ToTable("ImportDump");

        builder.HasIndex(e => new {e.CompanyId, e.SessionId}, "IDX_SessionID_CompanyId");

        builder.Property(e => e.EncryptedEmail)
            .HasMaxLength(150)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.GroupId).HasDefaultValueSql("((0))");

        builder.Property(e => e.Isd).HasColumnName("ISD");

        builder.Property(e => e.Llisd).HasColumnName("LLISD");

        builder.Property(e => e.LocLat)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.LocLng)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.LocationId).HasDefaultValueSql("((0))");

        builder.Property(e => e.SessionId).HasMaxLength(100);
    }
}