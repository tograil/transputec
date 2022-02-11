using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("MessageAttachment");

        builder.Property(e => e.MessageAttachmentId).HasColumnName("MessageAttachmentID");

        builder.Property(e => e.FilePath).HasMaxLength(500);

        builder.Property(e => e.FileSize).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.MessageListId).HasColumnName("MessageListID");

        builder.Property(e => e.OriginalFileName).HasMaxLength(120);

        builder.Property(e => e.Title).HasMaxLength(250);
    }
}