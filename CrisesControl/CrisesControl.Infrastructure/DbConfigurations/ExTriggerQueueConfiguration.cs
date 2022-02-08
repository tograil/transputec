using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ExTriggerQueueConfiguration : IEntityTypeConfiguration<ExTriggerQueue>
{
    public void Configure(EntityTypeBuilder<ExTriggerQueue> builder)
    {
        builder.ToTable("ExTriggerQueue");

        builder.Property(e => e.ExTriggerQueueId).HasColumnName("ExTriggerQueueID");

        builder.Property(e => e.EmailGuid).HasMaxLength(100);

        builder.Property(e => e.SourceFrom).HasMaxLength(150);

        builder.Property(e => e.TriggerKey).HasMaxLength(100);
    }
}