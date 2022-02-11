using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AppUserLogConfiguration : IEntityTypeConfiguration<AppUserLog>
{
    public void Configure(EntityTypeBuilder<AppUserLog> builder)
    {
        builder.HasKey(e => e.LogId)
                .HasName("PK_dbo.AppUserLog");

        builder.ToTable("AppUserLog");

        builder.Property(e => e.ControllerName).HasMaxLength(50);

        builder.Property(e => e.DeviceId).HasMaxLength(300);

        builder.Property(e => e.Lat).HasMaxLength(20);

        builder.Property(e => e.Lng).HasMaxLength(20);

        builder.Property(e => e.MethodName).HasMaxLength(100);
    }
}