using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SopheaderConfiguration : IEntityTypeConfiguration<Sopheader>
{
    public void Configure(EntityTypeBuilder<Sopheader> builder)
    {
        builder.ToTable("SOPHeader");

        builder.Property(e => e.SopheaderId).HasColumnName("SOPHeaderID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.ReviewFrequency).HasMaxLength(20);

        builder.Property(e => e.Sopowner).HasColumnName("SOPOwner");

        builder.Property(e => e.Sopversion)
            .HasMaxLength(20)
            .HasColumnName("SOPVersion");
    }
}