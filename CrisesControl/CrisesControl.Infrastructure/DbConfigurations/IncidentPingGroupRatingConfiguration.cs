using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentPingGroupRatingConfiguration : IEntityTypeConfiguration<IncidentPingGroupRating>
{
    public void Configure(EntityTypeBuilder<IncidentPingGroupRating> builder)
    {
        builder.HasNoKey();

        builder.ToView("IncidentPingGroupRating");

        builder.Property(e => e.MessageDate).HasColumnType("datetime");

        builder.Property(e => e.NewGroupId).HasColumnName("NewGroupID");

        builder.Property(e => e.TotalIncidentAck).HasColumnName("TotalIncidentACK");

        builder.Property(e => e.TotalIncidentAfterKpimax).HasColumnName("TotalIncidentAfterKPIMax");

        builder.Property(e => e.TotalIncidentInKpi).HasColumnName("TotalIncidentInKPI");

        builder.Property(e => e.TotalIncidentInKpimax).HasColumnName("TotalIncidentInKPIMax");

        builder.Property(e => e.TotalPingAck).HasColumnName("TotalPingACK");

        builder.Property(e => e.TotalPingAfterKpimax).HasColumnName("TotalPingAfterKPIMax");

        builder.Property(e => e.TotalPingInKpi).HasColumnName("TotalPingInKPI");

        builder.Property(e => e.TotalPingInKpimax).HasColumnName("TotalPingInKPIMax");

        builder.Property(e => e.Trtype)
            .HasMaxLength(8)
            .IsUnicode(false);
    }
}