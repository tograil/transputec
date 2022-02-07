using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ExceptionLogConfiguration : IEntityTypeConfiguration<ExceptionLog>
{
    public void Configure(EntityTypeBuilder<ExceptionLog> builder)
    {
        builder.ToTable("ExceptionLog");

        builder.Property(e => e.ExceptionLogId).HasColumnName("ExceptionLogID");

        builder.Property(e => e.ControllerName).HasMaxLength(100);

        builder.Property(e => e.ErrorId)
            .HasMaxLength(500)
            .HasColumnName("ErrorID");

        builder.Property(e => e.MethodName).HasMaxLength(100);

    }
}