using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ProcessQueueConfiguration : IEntityTypeConfiguration<ProcessQueue>
{
    public void Configure(EntityTypeBuilder<ProcessQueue> builder)
    {
        builder.HasKey(e => e.QueueId)
            .HasName("PK_dbo.ProcessQueue");

        builder.ToTable("ProcessQueue");

        builder.Property(e => e.MessageType).HasMaxLength(50);

        builder.Property(e => e.Method).HasMaxLength(20);

        builder.Property(e => e.State).HasMaxLength(20);
    }
}