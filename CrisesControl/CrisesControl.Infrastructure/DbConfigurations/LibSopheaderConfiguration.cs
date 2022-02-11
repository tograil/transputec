using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibSopheaderConfiguration : IEntityTypeConfiguration<LibSopheader>
{
    public void Configure(EntityTypeBuilder<LibSopheader> builder)
    {
        builder.ToTable("LibSOPHeader");

        builder.Property(e => e.LibSopheaderId).HasColumnName("LibSOPHeaderID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");

        builder.Property(e => e.Sopversion)
            .HasMaxLength(20)
            .HasColumnName("SOPVersion");
    }
}