using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ModelMappingLogConfiguration : IEntityTypeConfiguration<ModelMappingLog>
{
    public void Configure(EntityTypeBuilder<ModelMappingLog> builder)
    {
        builder.ToTable("ModelMappingLog");

        builder.Property(e => e.ControllerName).HasMaxLength(250);

        builder.Property(e => e.MethodName).HasMaxLength(250);
    }
}