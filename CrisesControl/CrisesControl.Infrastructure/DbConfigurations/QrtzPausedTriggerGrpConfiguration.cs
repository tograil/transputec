using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzPausedTriggerGrpConfiguration : IEntityTypeConfiguration<QrtzPausedTriggerGrp>
{
    public void Configure(EntityTypeBuilder<QrtzPausedTriggerGrp> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.TriggerGroup});

        builder.ToTable("QRTZ_PAUSED_TRIGGER_GRPS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.TriggerGroup)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_GROUP");
    }
}