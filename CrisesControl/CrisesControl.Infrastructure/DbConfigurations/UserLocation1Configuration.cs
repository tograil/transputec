using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserLocation1Configuration : IEntityTypeConfiguration<UserLocation1>
{
    public void Configure(EntityTypeBuilder<UserLocation1> builder)
    {
        builder.HasKey(e => e.UserLocationId)
            .HasName("PK_dbo.UserLocation");

        builder.ToTable("UserLocation");

        builder.Property(e => e.UserLocationId).HasColumnName("UserLocationID");

        builder.Property(e => e.CreatedOnGmt).HasColumnName("CreatedOnGMT");

        builder.Property(e => e.LocationAddress).HasMaxLength(500);

        builder.Property(e => e.UserDeviceId).HasColumnName("UserDeviceID");

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}