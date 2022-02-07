using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TrackMeConfiguration : IEntityTypeConfiguration<TrackMe>
{
    public void Configure(EntityTypeBuilder<TrackMe> builder)
    {
        builder.ToTable("TrackMe");

        builder.Property(e => e.TrackMeId).HasColumnName("TrackMeID");

        builder.Property(e => e.ActiveIncidentId).HasColumnName("ActiveIncidentID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.TrackType)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.UserDeviceId).HasColumnName("UserDeviceID");

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}