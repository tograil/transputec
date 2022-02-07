using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class FailedLoginAttemptConfiguration : IEntityTypeConfiguration<FailedLoginAttempt>
{
    public void Configure(EntityTypeBuilder<FailedLoginAttempt> builder)
    {
        builder.ToTable("FailedLoginAttempt");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.CustomerId).HasMaxLength(50);

        builder.Property(e => e.EmailId).HasMaxLength(150);

        builder.Property(e => e.Ipaddress)
            .HasMaxLength(250)
            .HasColumnName("IPAddress");

        builder.Property(e => e.PasswordUsed).HasMaxLength(150);
    }
}