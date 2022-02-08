using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyKpiConfiguration : IEntityTypeConfiguration<CompanyKpi>
{
    public void Configure(EntityTypeBuilder<CompanyKpi> builder)
    {
        builder.HasNoKey();

        builder.ToView("CompanyKPI");

        builder.Property(e => e.IncidentKpi)
                .HasMaxLength(500)
                .HasColumnName("IncidentKPI");

        builder.Property(e => e.IncidentKpimax)
                .HasMaxLength(500)
                .HasColumnName("IncidentKPIMax");

        builder.Property(e => e.PingKpi)
                .HasMaxLength(500)
                .HasColumnName("PingKPI");

        builder.Property(e => e.PingKpimax)
                .HasMaxLength(500)
                .HasColumnName("PingKPIMax");
    }
}