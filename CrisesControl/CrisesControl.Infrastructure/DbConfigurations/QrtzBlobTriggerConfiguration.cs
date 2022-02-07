using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzBlobTriggerConfiguration : IEntityTypeConfiguration<QrtzBlobTrigger>
{
    public void Configure(EntityTypeBuilder<QrtzBlobTrigger> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.TriggerName, e.TriggerGroup});

        builder.ToTable("QRTZ_BLOB_TRIGGERS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.TriggerName)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_NAME");

        builder.Property(e => e.TriggerGroup)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_GROUP");

        builder.Property(e => e.BlobData)
            .HasColumnType("image")
            .HasColumnName("BLOB_DATA");
    }
}