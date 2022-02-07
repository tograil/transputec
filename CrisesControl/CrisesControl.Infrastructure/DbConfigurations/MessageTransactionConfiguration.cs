using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MessageTransactionConfiguration : IEntityTypeConfiguration<MessageTransaction>
{
    public void Configure(EntityTypeBuilder<MessageTransaction> builder)
    {
        builder.ToTable("MessageTransaction");

        builder.HasIndex(e => e.CloudMessageId, "IDX_CloudMessageId");

        builder.HasIndex(e => e.IsBilled, "IDX_IsBilled");

        builder.HasIndex(e => e.LogCollected, "IDX_LogCollected");

        builder.HasIndex(e => e.MessageId, "IDX_MessageTransaction_MessageID");

        builder.HasIndex(e => e.MessageListId, "IDX_MessageTransaction_MessageListID");

        builder.HasIndex(e => e.MethodName, "IDX_MethodName");

        builder.HasIndex(e => e.Status, "IDX_Status");

        builder.Property(e => e.CloudMessageId).HasMaxLength(100);

        builder.Property(e => e.CommsProvider)
            .HasMaxLength(25)
            .IsUnicode(false);

        builder.Property(e => e.DeviceAddress).HasMaxLength(50);

        builder.Property(e => e.MethodName).HasMaxLength(20);

        builder.Property(e => e.Status).HasMaxLength(20);
    }
}