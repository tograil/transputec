using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentStatConfiguration : IEntityTypeConfiguration<IncidentStat>
{
    public void Configure(EntityTypeBuilder<IncidentStat> builder)
    {
        builder.HasNoKey();

        builder.ToView("IncidentStats");

        builder.Property(e => e.TotalIncidentAck).HasColumnName("TotalIncidentACK");

        builder.Property(e => e.TotalIncidentInKpi).HasColumnName("TotalIncidentInKPI");

        builder.Property(e => e.TotalIncidentNotInKpi).HasColumnName("TotalIncidentNotInKPI");

        builder.Property(e => e.TotalIncidentNotInKpimax).HasColumnName("TotalIncidentNotInKPIMax");
    }
}