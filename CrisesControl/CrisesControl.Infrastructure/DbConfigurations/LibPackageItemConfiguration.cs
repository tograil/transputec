using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibPackageItemConfiguration : IEntityTypeConfiguration<LibPackageItem>
{
    public void Configure(EntityTypeBuilder<LibPackageItem> builder)
    {
        builder.ToTable("LibPackageItem");

        builder.Property(e => e.ItemCode).HasMaxLength(50);

        builder.Property(e => e.ItemDescription).HasMaxLength(250);

        builder.Property(e => e.ItemName).HasMaxLength(100);

        builder.Property(e => e.ItemValue).HasMaxLength(150);
    }
}