using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TagCategoryConfiguration : IEntityTypeConfiguration<TagCategory>
{
    public void Configure(EntityTypeBuilder<TagCategory> builder)
    {
        builder.ToTable("TagCategory");

        builder.Property(e => e.TagCategoryId).HasColumnName("TagCategoryID");

        builder.Property(e => e.TagCategoryName).HasMaxLength(100);

        builder.Property(e => e.TagCategorySearchTerms).HasMaxLength(250);
    }
}