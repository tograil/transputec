using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyPackageFeatureConfiguration : IEntityTypeConfiguration<CompanyPackageFeature>
{
    public void Configure(EntityTypeBuilder<CompanyPackageFeature> builder)
    {
        builder.HasKey(e => e.PackageFeatureId)
            .HasName("PK_dbo.CompanyPackageFeature");

        builder.ToTable("CompanyPackageFeature");

        builder.Property(e => e.PackageFeatureId).HasColumnName("PackageFeatureID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.SecurityObjectId).HasColumnName("SecurityObjectID");
    }
}