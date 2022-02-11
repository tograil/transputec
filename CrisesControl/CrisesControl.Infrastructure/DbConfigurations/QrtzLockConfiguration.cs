using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzLockConfiguration : IEntityTypeConfiguration<QrtzLock>
{
    public void Configure(EntityTypeBuilder<QrtzLock> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.LockName});

        builder.ToTable("QRTZ_LOCKS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.LockName)
            .HasMaxLength(40)
            .HasColumnName("LOCK_NAME");
    }
}