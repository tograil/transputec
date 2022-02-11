using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserLoginLogConfiguration : IEntityTypeConfiguration<UserLoginLog>
{
    public void Configure(EntityTypeBuilder<UserLoginLog> builder)
    {
        builder.ToTable("UserLoginLog");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.UserId, "IDX_UserId")
            .HasFillFactor(100);

        builder.Property(e => e.DeviceType).HasMaxLength(300);

        builder.Property(e => e.Ipaddress)
            .HasMaxLength(20)
            .HasColumnName("IPAddress");
    }
}