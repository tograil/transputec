using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzFiredTriggerConfiguration : IEntityTypeConfiguration<QrtzFiredTrigger>
{
    public void Configure(EntityTypeBuilder<QrtzFiredTrigger> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.EntryId});

        builder.ToTable("QRTZ_FIRED_TRIGGERS");

        builder.HasIndex(e => new {e.SchedName, e.InstanceName, e.RequestsRecovery}, "IDX_QRTZ_FT_INST_JOB_REQ_RCVRY");

        builder.HasIndex(e => new {e.SchedName, e.JobGroup}, "IDX_QRTZ_FT_JG");

        builder.HasIndex(e => new {e.SchedName, e.JobName, e.JobGroup}, "IDX_QRTZ_FT_J_G");

        builder.HasIndex(e => new {e.SchedName, e.TriggerGroup}, "IDX_QRTZ_FT_TG");

        builder.HasIndex(e => new {e.SchedName, e.InstanceName}, "IDX_QRTZ_FT_TRIG_INST_NAME");

        builder.HasIndex(e => new {e.SchedName, e.TriggerName, e.TriggerGroup}, "IDX_QRTZ_FT_T_G");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.EntryId)
            .HasMaxLength(95)
            .HasColumnName("ENTRY_ID");

        builder.Property(e => e.FiredTime).HasColumnName("FIRED_TIME");

        builder.Property(e => e.InstanceName)
            .HasMaxLength(200)
            .HasColumnName("INSTANCE_NAME");

        builder.Property(e => e.IsNonconcurrent).HasColumnName("IS_NONCONCURRENT");

        builder.Property(e => e.JobGroup)
            .HasMaxLength(150)
            .HasColumnName("JOB_GROUP");

        builder.Property(e => e.JobName)
            .HasMaxLength(150)
            .HasColumnName("JOB_NAME");

        builder.Property(e => e.Priority).HasColumnName("PRIORITY");

        builder.Property(e => e.RequestsRecovery).HasColumnName("REQUESTS_RECOVERY");

        builder.Property(e => e.SchedTime).HasColumnName("SCHED_TIME");

        builder.Property(e => e.State)
            .HasMaxLength(16)
            .HasColumnName("STATE");

        builder.Property(e => e.TriggerGroup)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_GROUP");

        builder.Property(e => e.TriggerName)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_NAME");
    }
}