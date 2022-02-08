using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SysParameterConfiguration : IEntityTypeConfiguration<SysParameter>
{
    public void Configure(EntityTypeBuilder<SysParameter> builder)
    {
        builder.HasKey(e => e.SysParametersId)
            .HasName("PK_dbo.SysParameters");

        builder.HasIndex(e => e.Category, "IDX_Category")
            .HasFillFactor(100);

        builder.HasIndex(e => e.Name, "IDX_Name")
            .HasFillFactor(100);

        builder.Property(e => e.Category).HasMaxLength(70);

        builder.Property(e => e.Name).HasMaxLength(70);

        builder.Property(e => e.Type).HasMaxLength(100);
    }
}