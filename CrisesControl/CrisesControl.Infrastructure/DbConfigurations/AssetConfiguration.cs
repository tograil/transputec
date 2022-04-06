using CrisesControl.Core.AssetAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AssetConfiguration : IEntityTypeConfiguration<Assets>
{
    public void Configure(EntityTypeBuilder<Assets> builder)
    {
        builder.HasIndex(e => e.Status, "IDX_Status")
                .HasFillFactor(100);

        builder.HasIndex(e => e.CompanyId, "IX_CompanyId");

        builder.Property(e => e.AssetDescription).HasMaxLength(250);

        builder.Property(e => e.AssetPath).HasMaxLength(300);

        builder.Property(e => e.AssetTitle).HasMaxLength(250);

        builder.Property(e => e.AssetType).HasMaxLength(50);

        builder.Property(e => e.ReviewFrequency)
                .HasMaxLength(20)
                .IsUnicode(false);

        builder.Property(e => e.SourceFileName).HasMaxLength(150);
    }
}