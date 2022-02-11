using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentPingUserRatingConfiguration : IEntityTypeConfiguration<IncidentPingUserRating>
{
    public void Configure(EntityTypeBuilder<IncidentPingUserRating> builder)
    {
        builder.HasNoKey();

        builder.ToView("IncidentPingUserRating");

        builder.Property(e => e.TotalIncidentAck).HasColumnName("TotalIncidentACK");

        builder.Property(e => e.TotalIncidentInKpi).HasColumnName("TotalIncidentInKPI");

        builder.Property(e => e.TotalPingAck).HasColumnName("TotalPingACK");

        builder.Property(e => e.TotalPingInKpi).HasColumnName("TotalPingInKPI");
    }
}