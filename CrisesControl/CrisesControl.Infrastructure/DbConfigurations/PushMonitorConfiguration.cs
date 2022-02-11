using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PushMonitorConfiguration : IEntityTypeConfiguration<PushMonitor>
{
    public void Configure(EntityTypeBuilder<PushMonitor> builder)
    {
        builder.HasNoKey();

        builder.ToView("PushMonitor");

        builder.Property(e => e.Bbcount).HasColumnName("BBCount");

        builder.Property(e => e.Bbsuccess).HasColumnName("BBSuccess");

        builder.Property(e => e.IPhoneCount).HasColumnName("iPhoneCount");

        builder.Property(e => e.IPhoneSuccess).HasColumnName("iPhoneSuccess");

        builder.Property(e => e.MessageDate)
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(e => e.MessageTime)
            .HasMaxLength(5)
            .IsUnicode(false);
    }
}