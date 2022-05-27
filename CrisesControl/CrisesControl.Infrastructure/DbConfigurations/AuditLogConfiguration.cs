using CrisesControl.Core.AuditLog;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog");

        builder.Property(e => e.AuditLogId).ValueGeneratedNever();

        builder.Property(e => e.ColumnName).HasMaxLength(100);

        builder.Property(e => e.EventDateUtc).HasColumnName("EventDateUTC");

        builder.Property(e => e.EventType).HasMaxLength(1);

        builder.Property(e => e.TableName).HasMaxLength(100);
    }
}