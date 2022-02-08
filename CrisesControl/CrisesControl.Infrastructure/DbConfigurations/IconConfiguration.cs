using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IconConfiguration : IEntityTypeConfiguration<Icon>
{
    public void Configure(EntityTypeBuilder<Icon> builder)
    {
        builder.Property(e => e.IconId).HasColumnName("IconID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.IconFile).HasMaxLength(100);

        builder.Property(e => e.IconTags).HasMaxLength(250);

        builder.Property(e => e.IconTitle).HasMaxLength(100);
    }
}