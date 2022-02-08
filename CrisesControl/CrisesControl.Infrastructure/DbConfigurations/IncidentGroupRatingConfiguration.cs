using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentGroupRatingConfiguration : IEntityTypeConfiguration<IncidentGroupRating>
{
    public void Configure(EntityTypeBuilder<IncidentGroupRating> builder)
    {
        builder.HasNoKey();

        builder.ToView("IncidentGroupRating");

        builder.Property(e => e.MessageDate).HasColumnType("datetime");

        builder.Property(e => e.NewGroupId).HasColumnName("NewGroupID");

        builder.Property(e => e.TotalIncidentAck).HasColumnName("TotalIncidentACK");

        builder.Property(e => e.TotalIncidentAfterKpimax).HasColumnName("TotalIncidentAfterKPIMax");

        builder.Property(e => e.TotalIncidentInKpi).HasColumnName("TotalIncidentInKPI");

        builder.Property(e => e.TotalIncidentInKpimax).HasColumnName("TotalIncidentInKPIMax");
    }
}