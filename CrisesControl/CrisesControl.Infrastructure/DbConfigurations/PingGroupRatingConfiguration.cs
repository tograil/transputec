using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PingGroupRatingConfiguration : IEntityTypeConfiguration<PingGroupRating>
{
    public void Configure(EntityTypeBuilder<PingGroupRating> builder)
    {
        builder.HasNoKey();

        builder.ToView("PingGroupRating");

        builder.Property(e => e.MessageDate).HasColumnType("datetime");

        builder.Property(e => e.NewGroupId).HasColumnName("NewGroupID");

        builder.Property(e => e.TotalPingAck).HasColumnName("TotalPingACK");

        builder.Property(e => e.TotalPingAfterKpimax).HasColumnName("TotalPingAfterKPIMax");

        builder.Property(e => e.TotalPingInKpi).HasColumnName("TotalPingInKPI");

        builder.Property(e => e.TotalPingInKpimax).HasColumnName("TotalPingInKPIMax");
    }
}