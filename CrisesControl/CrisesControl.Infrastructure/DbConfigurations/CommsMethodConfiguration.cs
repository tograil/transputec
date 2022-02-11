using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CommsMethodConfiguration : IEntityTypeConfiguration<CommsMethod>
{
    public void Configure(EntityTypeBuilder<CommsMethod> builder)
    {
        builder.ToTable("CommsMethod");

        builder.Property(e => e.MethodCode).HasMaxLength(10);

        builder.Property(e => e.MethodName).HasMaxLength(50);
    }
}