using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentPingStatConfiguration : IEntityTypeConfiguration<IncidentPingStat>
{
    public void Configure(EntityTypeBuilder<IncidentPingStat> builder)
    {
        builder.HasNoKey();

        builder.ToView("IncidentPingStats");

        builder.Property(e => e.TotalIncidentAck).HasColumnName("TotalIncidentACK");

        builder.Property(e => e.TotalIncidentInKpi).HasColumnName("TotalIncidentInKPI");

        builder.Property(e => e.TotalIncidentOutKpi).HasColumnName("TotalIncidentOutKPI");

        builder.Property(e => e.TotalIncidentOutKpimax).HasColumnName("TotalIncidentOutKPIMax");

        builder.Property(e => e.TotalPingAck).HasColumnName("TotalPingACK");

        builder.Property(e => e.TotalPingInKpi).HasColumnName("TotalPingInKPI");

        builder.Property(e => e.TotalPingOutKpi).HasColumnName("TotalPingOutKPI");

        builder.Property(e => e.TotalPingOutKpimax).HasColumnName("TotalPingOutKPIMax");
    }
}