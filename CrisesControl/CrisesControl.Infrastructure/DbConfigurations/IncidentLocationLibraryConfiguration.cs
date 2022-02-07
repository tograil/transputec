using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentLocationLibraryConfiguration : IEntityTypeConfiguration<IncidentLocationLibrary>
{
    public void Configure(EntityTypeBuilder<IncidentLocationLibrary> builder)
    {
        builder.HasKey(e => e.LocationId);

        builder.ToTable("IncidentLocationLibrary");

        builder.Property(e => e.LocationId).HasColumnName("LocationID");

        builder.Property(e => e.Address).HasMaxLength(250);

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.Lat).HasMaxLength(15);

        builder.Property(e => e.Lng).HasMaxLength(15);

        builder.Property(e => e.LocationName).HasMaxLength(250);

        builder.Property(e => e.LocationType).HasMaxLength(20);
    }
}