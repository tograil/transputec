using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class JobHistoryConfiguration : IEntityTypeConfiguration<JobHistory>
{
    public void Configure(EntityTypeBuilder<JobHistory> builder)
    {
        builder.ToTable("JobHistory");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.JobId).HasColumnName("JobID");

        builder.Property(e => e.Message).HasMaxLength(250);

        builder.Property(e => e.PingMessageId).HasColumnName("PingMessageID");
    }
}