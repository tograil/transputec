using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzTriggerConfiguration : IEntityTypeConfiguration<QrtzTrigger>
{
    public void Configure(EntityTypeBuilder<QrtzTrigger> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.TriggerName, e.TriggerGroup});

        builder.ToTable("QRTZ_TRIGGERS");

        builder.HasIndex(e => new {e.SchedName, e.CalendarName}, "IDX_QRTZ_T_C");

        builder.HasIndex(e => new {e.SchedName, e.TriggerGroup}, "IDX_QRTZ_T_G");

        builder.HasIndex(e => new {e.SchedName, e.JobName, e.JobGroup}, "IDX_QRTZ_T_J");

        builder.HasIndex(e => new {e.SchedName, e.JobGroup}, "IDX_QRTZ_T_JG");

        builder.HasIndex(e => new {e.SchedName, e.NextFireTime}, "IDX_QRTZ_T_NEXT_FIRE_TIME");

        builder.HasIndex(e => new {e.SchedName, e.MisfireInstr, e.NextFireTime}, "IDX_QRTZ_T_NFT_MISFIRE");

        builder.HasIndex(e => new {e.SchedName, e.TriggerState, e.NextFireTime}, "IDX_QRTZ_T_NFT_ST");

        builder.HasIndex(e => new {e.SchedName, e.MisfireInstr, e.NextFireTime, e.TriggerState},
            "IDX_QRTZ_T_NFT_ST_MISFIRE");

        builder.HasIndex(e => new {e.SchedName, e.MisfireInstr, e.NextFireTime, e.TriggerGroup, e.TriggerState},
            "IDX_QRTZ_T_NFT_ST_MISFIRE_GRP");

        builder.HasIndex(e => new {e.SchedName, e.TriggerGroup, e.TriggerState}, "IDX_QRTZ_T_N_G_STATE");

        builder.HasIndex(e => new {e.SchedName, e.TriggerName, e.TriggerGroup, e.TriggerState}, "IDX_QRTZ_T_N_STATE");

        builder.HasIndex(e => new {e.SchedName, e.TriggerState}, "IDX_QRTZ_T_STATE");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.TriggerName)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_NAME");

        builder.Property(e => e.TriggerGroup)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_GROUP");

        builder.Property(e => e.CalendarName)
            .HasMaxLength(200)
            .HasColumnName("CALENDAR_NAME");

        builder.Property(e => e.Description)
            .HasMaxLength(250)
            .HasColumnName("DESCRIPTION");

        builder.Property(e => e.EndTime).HasColumnName("END_TIME");

        builder.Property(e => e.JobData)
            .HasColumnType("image")
            .HasColumnName("JOB_DATA");

        builder.Property(e => e.JobGroup)
            .HasMaxLength(150)
            .HasColumnName("JOB_GROUP");

        builder.Property(e => e.JobName)
            .HasMaxLength(150)
            .HasColumnName("JOB_NAME");

        builder.Property(e => e.MisfireInstr).HasColumnName("MISFIRE_INSTR");

        builder.Property(e => e.NextFireTime).HasColumnName("NEXT_FIRE_TIME");

        builder.Property(e => e.PrevFireTime).HasColumnName("PREV_FIRE_TIME");

        builder.Property(e => e.Priority).HasColumnName("PRIORITY");

        builder.Property(e => e.StartTime).HasColumnName("START_TIME");

        builder.Property(e => e.TriggerState)
            .HasMaxLength(16)
            .HasColumnName("TRIGGER_STATE");

        builder.Property(e => e.TriggerType)
            .HasMaxLength(8)
            .HasColumnName("TRIGGER_TYPE");

        builder.HasOne(d => d.QrtzJobDetail)
            .WithMany(p => p.QrtzTriggers)
            .HasForeignKey(d => new {d.SchedName, d.JobName, d.JobGroup})
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS");
    }
}