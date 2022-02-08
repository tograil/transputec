using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzSchedulerStateConfiguration : IEntityTypeConfiguration<QrtzSchedulerState>
{
    public void Configure(EntityTypeBuilder<QrtzSchedulerState> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.InstanceName});

        builder.ToTable("QRTZ_SCHEDULER_STATE");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.InstanceName)
            .HasMaxLength(200)
            .HasColumnName("INSTANCE_NAME");

        builder.Property(e => e.CheckinInterval).HasColumnName("CHECKIN_INTERVAL");

        builder.Property(e => e.LastCheckinTime).HasColumnName("LAST_CHECKIN_TIME");
    }
}