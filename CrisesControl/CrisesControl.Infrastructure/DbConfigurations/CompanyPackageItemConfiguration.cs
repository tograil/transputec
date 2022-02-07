using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyPackageItemConfiguration : IEntityTypeConfiguration<CompanyPackageItem>
{
    public void Configure(EntityTypeBuilder<CompanyPackageItem> builder)
    {
        builder.ToTable("CompanyPackageItem");

        builder.Property(e => e.ItemCode).HasMaxLength(50);

        builder.Property(e => e.ItemValue).HasMaxLength(150);
    }
}