using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SosimpactedLocationConfiguration : IEntityTypeConfiguration<SosimpactedLocation>
{
    public void Configure(EntityTypeBuilder<SosimpactedLocation> builder)
    {
        builder.HasKey(e => e.LocationId);

        builder.ToTable("SOSImpactedLocation");

        builder.Property(e => e.LocationId).HasColumnName("LocationID");

        builder.Property(e => e.ImpactedLocationId).HasColumnName("ImpactedLocationID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");
    }
}