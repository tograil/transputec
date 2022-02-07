using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class Log4NetLogConfiguration : IEntityTypeConfiguration<Log4NetLog>
{
    public void Configure(EntityTypeBuilder<Log4NetLog> builder)
    {
        builder.ToTable("Log4NetLog");

        builder.Property(e => e.ControllerName)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Date).HasColumnType("datetime");

        builder.Property(e => e.Level)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.Logger)
            .HasMaxLength(255)
            .IsUnicode(false);

        builder.Property(e => e.Message).IsUnicode(false);

        builder.Property(e => e.MethodName)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Thread)
            .HasMaxLength(255)
            .IsUnicode(false);
    }
}