using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzSimpropTriggerConfiguration : IEntityTypeConfiguration<QrtzSimpropTrigger>
{
    public void Configure(EntityTypeBuilder<QrtzSimpropTrigger> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.TriggerName, e.TriggerGroup});

        builder.ToTable("QRTZ_SIMPROP_TRIGGERS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.TriggerName)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_NAME");

        builder.Property(e => e.TriggerGroup)
            .HasMaxLength(150)
            .HasColumnName("TRIGGER_GROUP");

        builder.Property(e => e.BoolProp1).HasColumnName("BOOL_PROP_1");

        builder.Property(e => e.BoolProp2).HasColumnName("BOOL_PROP_2");

        builder.Property(e => e.DecProp1)
            .HasColumnType("numeric(13, 4)")
            .HasColumnName("DEC_PROP_1");

        builder.Property(e => e.DecProp2)
            .HasColumnType("numeric(13, 4)")
            .HasColumnName("DEC_PROP_2");

        builder.Property(e => e.IntProp1).HasColumnName("INT_PROP_1");

        builder.Property(e => e.IntProp2).HasColumnName("INT_PROP_2");

        builder.Property(e => e.LongProp1).HasColumnName("LONG_PROP_1");

        builder.Property(e => e.LongProp2).HasColumnName("LONG_PROP_2");

        builder.Property(e => e.StrProp1)
            .HasMaxLength(512)
            .HasColumnName("STR_PROP_1");

        builder.Property(e => e.StrProp2)
            .HasMaxLength(512)
            .HasColumnName("STR_PROP_2");

        builder.Property(e => e.StrProp3)
            .HasMaxLength(512)
            .HasColumnName("STR_PROP_3");

        builder.HasOne(d => d.QrtzTrigger)
            .WithOne(p => p.QrtzSimpropTrigger)
            .HasForeignKey<QrtzSimpropTrigger>(d => new {d.SchedName, d.TriggerName, d.TriggerGroup})
            .HasConstraintName("FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS");
    }
}