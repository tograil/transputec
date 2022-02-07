using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PublicAlertMessageListDumpConfiguration : IEntityTypeConfiguration<PublicAlertMessageListDump>
{
    public void Configure(EntityTypeBuilder<PublicAlertMessageListDump> builder)
    {
        builder.HasKey(e => e.QueueId);

        builder.ToTable("PublicAlertMessageListDump");

        builder.Property(e => e.QueueId).HasColumnName("QueueID");

        builder.Property(e => e.EmailId)
            .HasMaxLength(150)
            .HasColumnName("EmailID");

        builder.Property(e => e.Latitude).HasMaxLength(15);

        builder.Property(e => e.Longitude).HasMaxLength(15);

        builder.Property(e => e.MobileNo).HasMaxLength(20);

        builder.Property(e => e.Postcode).HasMaxLength(20);

        builder.Property(e => e.PublicAlertId).HasColumnName("PublicAlertID");

        builder.Property(e => e.SessionId).HasMaxLength(100);
    }
}