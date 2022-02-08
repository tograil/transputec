using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MessageProcessLogConfiguration : IEntityTypeConfiguration<MessageProcessLog>
{
    public void Configure(EntityTypeBuilder<MessageProcessLog> builder)
    {
        builder.HasKey(e => e.LogId);

        builder.ToTable("MessageProcessLog");

        builder.Property(e => e.AdditionalInfo)
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(e => e.EventName)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.MethodName)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.QueueName)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}