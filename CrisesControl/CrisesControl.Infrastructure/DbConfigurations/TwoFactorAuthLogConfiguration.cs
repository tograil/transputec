using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TwoFactorAuthLogConfiguration : IEntityTypeConfiguration<TwoFactorAuthLog>
{
    public void Configure(EntityTypeBuilder<TwoFactorAuthLog> builder)
    {
        builder.ToTable("TwoFactorAuthLog");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.CloudMessageId)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Status)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.ToNumber)
            .HasMaxLength(20);
    }
}