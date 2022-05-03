using CrisesControl.Core.Jobs;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class JobScheduleConfiguration : IEntityTypeConfiguration<JobSchedule>
{
    public void Configure(EntityTypeBuilder<JobSchedule> builder)
    {
        builder.HasKey(e => e.ScheduleId)
            .HasName("PK_dbo.JobSchedules");

        builder.HasIndex(e => e.JobId, "IX_JobID");

        builder.Property(e => e.ScheduleId).HasColumnName("ScheduleID");

        builder.Property(e => e.ActiveEndTime).HasMaxLength(10);

        builder.Property(e => e.ActiveStartTime).HasMaxLength(8);

        builder.Property(e => e.FrequencySubDayType).HasMaxLength(50);

        builder.Property(e => e.FrequencyType).HasMaxLength(50);

        builder.Property(e => e.JobId).HasColumnName("JobID");

        builder.Property(e => e.NextRunTime).HasMaxLength(10);

        builder.ToTable("JobSchedules");
    }
}