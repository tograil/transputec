using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ConferenceCallLogHeaderConfiguration : IEntityTypeConfiguration<ConferenceCallLogHeader>
{
    public void Configure(EntityTypeBuilder<ConferenceCallLogHeader> builder)
    {
        builder.HasKey(e => e.ConferenceCallId)
            .HasName("PK_dbo.ConferenceCallLogHeader");

        builder.ToTable("ConferenceCallLogHeader");

        builder.Property(e => e.CloudConfId).HasMaxLength(100);

        builder.Property(e => e.ConfRoomName).HasMaxLength(100);

        builder.Property(e => e.ConfrenceEnd).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.ConfrenceStart).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.CurrentStatus).HasMaxLength(50);

        builder.Property(e => e.RecordingSid).HasMaxLength(100);

        builder.Property(e => e.RecordingUrl)
            .HasMaxLength(500)
            .HasColumnName("RecordingURL");

        builder.Property(e => e.TargetObjectName).HasMaxLength(20);
    }
}