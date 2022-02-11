using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SecurityObjectTypeConfiguration : IEntityTypeConfiguration<SecurityObjectType>
{
    public void Configure(EntityTypeBuilder<SecurityObjectType> builder)
    {
        builder.ToTable("SecurityObjectType");

        builder.HasIndex(e => e.Code, "IDX_Code")
            .HasFillFactor(100);

        builder.Property(e => e.Code).HasMaxLength(50);

        builder.Property(e => e.Description).HasMaxLength(150);
    }
}