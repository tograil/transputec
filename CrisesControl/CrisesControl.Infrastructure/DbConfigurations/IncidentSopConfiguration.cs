using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentSopConfiguration : IEntityTypeConfiguration<IncidentSop>
{
    public void Configure(EntityTypeBuilder<IncidentSop> builder)
    {
        builder.ToTable("IncidentSOP");

        builder.Property(e => e.IncidentSopid).HasColumnName("IncidentSOPID");

        builder.Property(e => e.AssetId).HasColumnName("AssetID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");

        builder.Property(e => e.SopheaderId).HasColumnName("SOPHeaderID");
    }
}