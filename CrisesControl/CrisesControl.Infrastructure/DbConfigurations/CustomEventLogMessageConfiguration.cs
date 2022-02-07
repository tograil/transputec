using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CustomEventLogMessageConfiguration : IEntityTypeConfiguration<CustomEventLogMessage>
{
    public void Configure(EntityTypeBuilder<CustomEventLogMessage> builder)
    {
        builder.HasKey(e => e.EventLogMessageId);

        builder.ToTable("CustomEventLogMessage");

        builder.Property(e => e.EventLogMessageId).HasColumnName("EventLogMessageID");

        builder.Property(e => e.EventLogId).HasColumnName("EventLogID");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.RcpntGroupId).HasColumnName("RcpntGroupID");

        builder.Property(e => e.RcpntUserId).HasColumnName("RcpntUserID");

    }
}