using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PublicAlertMessageListConfiguration : IEntityTypeConfiguration<PublicAlertMessageList>
{
    public void Configure(EntityTypeBuilder<PublicAlertMessageList> builder)
    {
        builder.HasKey(e => e.QueueId)
            .HasName("PK_PublicAlertQueue");

        builder.ToTable("PublicAlertMessageList");

        builder.Property(e => e.QueueId).HasColumnName("QueueID");

        builder.Property(e => e.CloudMessageId)
            .HasMaxLength(100)
            .HasColumnName("CloudMessageID");

        builder.Property(e => e.EmailId)
            .HasMaxLength(150)
            .HasColumnName("EmailID");

        builder.Property(e => e.Latitude).HasMaxLength(15);

        builder.Property(e => e.Longitude).HasMaxLength(15);

        builder.Property(e => e.MessageDelvStatus).HasDefaultValueSql("((0))");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.MobileNo).HasMaxLength(20);

        builder.Property(e => e.Postcode).HasMaxLength(20);

        builder.Property(e => e.PublicAlertId).HasColumnName("PublicAlertID");
    }
}