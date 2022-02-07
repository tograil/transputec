using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MessageListConfiguration : IEntityTypeConfiguration<MessageList>
{
    public void Configure(EntityTypeBuilder<MessageList> builder)
    {
        builder.ToTable("MessageList");

        builder.HasIndex(e => e.DateSent, "IDX_DateSent")
            .HasFillFactor(100);

        builder.HasIndex(e => e.MessageDelvStatus, "IDX_MessageDelvStatus")
            .HasFillFactor(100);

        builder.HasIndex(e => e.MessageAckStatus, "IDX_MessageList_AckStatus");

        builder.HasIndex(e => new {e.RecepientUserId, e.MessageSentStatus, e.MessageAckStatus},
            "IDX_MessageList_RecepientUserID");

        builder.HasIndex(e => e.MessageSentStatus, "IDX_MessageList_SentStatus");

        builder.HasIndex(e => e.MessageAckStatus, "IDX_MessagesAckStatus");

        builder.HasIndex(e => e.RecepientUserId, "IDX_RecepientUserId");

        builder.HasIndex(e => e.MessageSentStatus, "IX_MessageSentStatus");

        builder.HasIndex(e => e.MessageId, "IX_MessgeID");

        builder.Property(e => e.AckMethod).HasMaxLength(10);

        builder.Property(e => e.ResponseId).HasColumnName("ResponseID");

        builder.Property(e => e.TransportType).HasMaxLength(50);

        builder.Property(e => e.UserLocationLat).HasMaxLength(150);

        builder.Property(e => e.UserLocationLong).HasMaxLength(150);
    }
}