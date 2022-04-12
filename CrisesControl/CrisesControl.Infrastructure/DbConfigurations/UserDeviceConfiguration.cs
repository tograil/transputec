using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.DeviceSerial, "IDX_DeviceSerial");

        builder.HasIndex(e => e.Status, "IDX_Status");

        builder.HasIndex(e => e.UserId, "IX_UserID");

        builder.Property(e => e.UserDeviceId).HasColumnName("UserDeviceID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.DeviceId)
            .HasMaxLength(300)
            .HasColumnName("DeviceID");

        builder.Property(e => e.DeviceModel).HasMaxLength(100);

        builder.Property(e => e.DeviceOs)
            .HasMaxLength(50)
            .HasColumnName("DeviceOS");

        builder.Property(e => e.DeviceSerial)
            .HasMaxLength(250)
            .HasDefaultValueSql("('')");

        builder.Property(e => e.DeviceToken).HasMaxLength(100);

        builder.Property(e => e.DeviceType).HasMaxLength(50);

        builder.Property(e => e.ExtraInfo).HasMaxLength(250);

        builder.Property(e => e.SirenOn).HasColumnName("SirenON");

        builder.Property(e => e.SoundFile)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasDefaultValueSql("('default')");

        builder.Property(e => e.UserId).HasColumnName("UserID");
        builder.ToTable("UserDevices");
    }
}