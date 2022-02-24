using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PackagePlanConfiguration : IEntityTypeConfiguration<PackagePlan>
{
    public void Configure(EntityTypeBuilder<PackagePlan> builder)
    {
        builder.ToTable("PackagePlan");

        builder.Property(e => e.PackagePrice).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.PlanDescription).HasMaxLength(250);

        builder.Property(e => e.PlanName).HasMaxLength(100);
    }
}