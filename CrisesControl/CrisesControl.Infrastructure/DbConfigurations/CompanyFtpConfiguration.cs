using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyFtpConfiguration : IEntityTypeConfiguration<CompanyFtp>
{
    public void Configure(EntityTypeBuilder<CompanyFtp> builder)
    {
        builder.ToTable("CompanyFTP");

        builder.Property(e => e.HostName).HasMaxLength(250);

        builder.Property(e => e.LogonType).HasMaxLength(100);

        builder.Property(e => e.Protocol).HasMaxLength(20);

        builder.Property(e => e.RemotePath).HasMaxLength(500);

        builder.Property(e => e.SecurityKey).HasMaxLength(500);

        builder.Property(e => e.ShafingerPrint).HasColumnName("SHAFingerPrint");

        builder.Property(e => e.UserName).HasMaxLength(250);
    }
}