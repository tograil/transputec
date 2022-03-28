using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentLocationConfiguration : IEntityTypeConfiguration<IncidentLocation>
{
    public void Configure(EntityTypeBuilder<IncidentLocation> builder)
    {
        builder.HasKey(e => e.LocationId);

        builder.ToTable("IncidentLocation");

        builder.Property(e => e.LocationId).HasColumnName("LocationID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.ImpactedLocationId).HasColumnName("ImpactedLocationID");

        builder.Property(e => e.IncidentActivationId).HasColumnName("IncidentActivationID");

        builder.Property(e => e.LibLocationId).HasColumnName("LibLocationID");

        builder.HasOne<IncidentLocationLibrary>().WithMany().HasForeignKey(e => e.LibLocationId);
    }
}