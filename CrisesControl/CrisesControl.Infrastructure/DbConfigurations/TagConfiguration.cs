using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tag");

        builder.Property(e => e.TagId).HasColumnName("TagID");

        builder.Property(e => e.SearchTerms).HasMaxLength(250);

        builder.Property(e => e.TagCategoryId).HasColumnName("TagCategoryID");

        builder.Property(e => e.TagName).HasMaxLength(100);
        builder.HasOne(e => e.LibContentTag).WithMany().HasForeignKey(e=>e.TagId);
    }
}