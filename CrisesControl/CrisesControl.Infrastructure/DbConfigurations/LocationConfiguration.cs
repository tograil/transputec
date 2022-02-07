using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Location");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.Status, "IDX_Status")
            .HasFillFactor(100);

        builder.Property(e => e.Desc).HasMaxLength(250);

        builder.Property(e => e.Lat).HasMaxLength(20);

        builder.Property(e => e.LocationName)
            .HasMaxLength(150)
            .HasColumnName("Location_Name");

        builder.Property(e => e.Long).HasMaxLength(20);

        builder.Property(e => e.PostCode).HasMaxLength(250);
    }
}