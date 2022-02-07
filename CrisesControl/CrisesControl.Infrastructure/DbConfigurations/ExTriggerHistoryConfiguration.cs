using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ExTriggerHistoryConfiguration : IEntityTypeConfiguration<ExTriggerHistory>
{
    public void Configure(EntityTypeBuilder<ExTriggerHistory> builder)
    {
        builder.ToTable("ExTriggerHistory");

        builder.HasIndex(e => e.ExTriggerId, "IX_ExTriggerID");

        builder.Property(e => e.ExTriggerHistoryId).HasColumnName("ExTriggerHistoryID");

        builder.Property(e => e.ActionApplied).HasMaxLength(100);

        builder.Property(e => e.ExTriggerId).HasColumnName("ExTriggerID");

        builder.Property(e => e.IncidentActivationId).HasColumnName("IncidentActivationID");

        builder.Property(e => e.PingMessageId).HasColumnName("PingMessageID");
    }
}