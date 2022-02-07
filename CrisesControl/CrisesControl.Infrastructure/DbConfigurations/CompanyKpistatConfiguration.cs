using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyKpistatConfiguration : IEntityTypeConfiguration<CompanyKpistat>
{
    public void Configure(EntityTypeBuilder<CompanyKpistat> builder)
    {
        builder.HasNoKey();

        builder.ToView("CompanyKPIStats");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.DateValue).HasColumnType("datetime");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.IncidentKpi)
                .HasMaxLength(500)
                .HasColumnName("IncidentKPI");

        builder.Property(e => e.IncidentMaxKpi)
                .HasMaxLength(500)
                .HasColumnName("IncidentMaxKPI");

        builder.Property(e => e.PingKpi)
                .HasMaxLength(500)
                .HasColumnName("PingKPI");

        builder.Property(e => e.PingMaxKpi)
                .HasMaxLength(500)
                .HasColumnName("PingMaxKPI");

        builder.Property(e => e.TotalIncidentAck).HasColumnName("TotalIncidentACK");

        builder.Property(e => e.TotalIncidentInKpi).HasColumnName("TotalIncidentInKPI");

        builder.Property(e => e.TotalIncidentNotAck).HasColumnName("TotalIncidentNotACK");

        builder.Property(e => e.TotalIncidentOutKpimax).HasColumnName("TotalIncidentOutKPIMax");

        builder.Property(e => e.TotalPingAck).HasColumnName("TotalPingACK");

        builder.Property(e => e.TotalPingInKpi).HasColumnName("TotalPingInKPI");

        builder.Property(e => e.TotalPingNotAck).HasColumnName("TotalPingNotACK");

        builder.Property(e => e.TotalPingOutKpimax).HasColumnName("TotalPingOutKPIMax");
    }
}