using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzSimpleTriggerConfiguration : IEntityTypeConfiguration<QrtzSimpleTrigger>
{
    public void Configure(EntityTypeBuilder<QrtzSimpleTrigger> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.TriggerName, e.TriggerGroup});

        builder.ToTable("QRTZ_SIMPLE_TRIGGERS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.TriggerName)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_NAME");

        builder.Property(e => e.TriggerGroup)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_GROUP");

        builder.Property(e => e.RepeatCount).HasColumnName("REPEAT_COUNT");

        builder.Property(e => e.RepeatInterval).HasColumnName("REPEAT_INTERVAL");

        builder.Property(e => e.TimesTriggered).HasColumnName("TIMES_TRIGGERED");

        builder.HasOne(d => d.QrtzTrigger)
            .WithOne(p => p.QrtzSimpleTrigger)
            .HasForeignKey<QrtzSimpleTrigger>(d => new {d.SchedName, d.TriggerName, d.TriggerGroup})
            .HasConstraintName("FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS");
    }
}