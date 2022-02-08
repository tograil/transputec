using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MethodsToLogConfiguration : IEntityTypeConfiguration<MethodsToLog>
{
    public void Configure(EntityTypeBuilder<MethodsToLog> builder)
    {
        builder.ToTable("MethodsToLog");

        builder.Property(e => e.ControllerName).HasMaxLength(100);

        builder.Property(e => e.MethodName).HasMaxLength(100);

        builder.Property(e => e.Status).HasDefaultValueSql("((1))");
    }
}