using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.Property(e => e.JobId).HasColumnName("JobID");

        builder.Property(e => e.ActionType).HasMaxLength(50);

        builder.Property(e => e.CommandLine).HasMaxLength(2000);

        builder.Property(e => e.JobDescription).HasMaxLength(500);

        builder.Property(e => e.JobIncidentId).HasColumnName("JobIncidentID");

        builder.Property(e => e.JobName).HasMaxLength(50);

        builder.Property(e => e.JobType).HasMaxLength(20);

        builder.Property(e => e.LastRunDateTime).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.LockedBy).HasMaxLength(100);

        builder.Property(e => e.NextRunTime).HasMaxLength(10);
    }
}