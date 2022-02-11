using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CommsMonitorConfiguration : IEntityTypeConfiguration<CommsMonitor>
{
    public void Configure(EntityTypeBuilder<CommsMonitor> builder)
    {
        builder.HasNoKey();

        builder.ToView("CommsMonitor");

        builder.Property(e => e.MessageDate)
                .HasMaxLength(30)
                .IsUnicode(false);

        builder.Property(e => e.MessageTime)
                .HasMaxLength(5)
                .IsUnicode(false);
    }
}