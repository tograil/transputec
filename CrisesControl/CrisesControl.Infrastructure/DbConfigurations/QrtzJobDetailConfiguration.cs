using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzJobDetailConfiguration : IEntityTypeConfiguration<QrtzJobDetail>
{
    public void Configure(EntityTypeBuilder<QrtzJobDetail> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.JobName, e.JobGroup});

        builder.ToTable("QRTZ_JOB_DETAILS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.JobName)
            .HasMaxLength(150)
            .HasColumnName("JOB_NAME");

        builder.Property(e => e.JobGroup)
            .HasMaxLength(150)
            .HasColumnName("JOB_GROUP");

        builder.Property(e => e.Description)
            .HasMaxLength(250)
            .HasColumnName("DESCRIPTION");

        builder.Property(e => e.IsDurable).HasColumnName("IS_DURABLE");

        builder.Property(e => e.IsNonconcurrent).HasColumnName("IS_NONCONCURRENT");

        builder.Property(e => e.IsUpdateData).HasColumnName("IS_UPDATE_DATA");

        builder.Property(e => e.JobClassName)
            .HasMaxLength(250)
            .HasColumnName("JOB_CLASS_NAME");

        builder.Property(e => e.JobData)
            .HasColumnType("image")
            .HasColumnName("JOB_DATA");

        builder.Property(e => e.RequestsRecovery).HasColumnName("REQUESTS_RECOVERY");
    }
}