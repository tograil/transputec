using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PackageFeatureConfiguration : IEntityTypeConfiguration<PackageFeature>
{
    public void Configure(EntityTypeBuilder<PackageFeature> builder)
    {
        builder.ToTable("PackageFeature");

        builder.HasIndex(e => e.SecurityObjectId, "IDX_SecurityObjectId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.PackagePlanId, "IX_PackagePlanId");
    }
}