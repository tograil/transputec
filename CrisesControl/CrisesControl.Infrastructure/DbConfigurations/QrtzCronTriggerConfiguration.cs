using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzCronTriggerConfiguration : IEntityTypeConfiguration<QrtzCronTrigger>
{
    public void Configure(EntityTypeBuilder<QrtzCronTrigger> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.TriggerName, e.TriggerGroup});

        builder.ToTable("QRTZ_CRON_TRIGGERS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.TriggerName)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_NAME");

        builder.Property(e => e.TriggerGroup)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_GROUP");

        builder.Property(e => e.CronExpression)
            .HasMaxLength(120)
            .HasColumnName("CRON_EXPRESSION");

        builder.Property(e => e.TimeZoneId)
            .HasMaxLength(80)
            .HasColumnName("TIME_ZONE_ID");

        builder.HasOne(d => d.QrtzTrigger)
            .WithOne(p => p.QrtzCronTrigger)
            .HasForeignKey<QrtzCronTrigger>(d => new {d.SchedName, d.TriggerName, d.TriggerGroup})
            .HasConstraintName("FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS");
    }
}