using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserLocationConfiguration : IEntityTypeConfiguration<UserLocation>
{
    public void Configure(EntityTypeBuilder<UserLocation> builder)
    {
        builder.HasNoKey();

        builder.ToView("User_Location");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.Desc).HasMaxLength(250);

        builder.Property(e => e.FirstName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.LastName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Lat).HasMaxLength(20);

        builder.Property(e => e.LocationName)
            .HasMaxLength(150)
            .HasColumnName("Location_Name");

        builder.Property(e => e.Long).HasMaxLength(20);

        builder.Property(e => e.PostCode).HasMaxLength(250);

        builder.Property(e => e.UniqueId).HasColumnName("UniqueID");

        builder.Property(e => e.UserId).HasColumnName("UserID");
        builder.Property(e => e.UserDeviceId).HasColumnName("UserDeviceID");
        builder.Property(e => e.CreatedOn).HasColumnName("CreatedOn");
    }
}