using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ContentTagConfiguration : IEntityTypeConfiguration<ContentTag>
{
    public void Configure(EntityTypeBuilder<ContentTag> builder)
    {
        builder.ToTable("ContentTag");

        builder.Property(e => e.ContentTagId).HasColumnName("ContentTagID");

        builder.Property(e => e.ContentId).HasColumnName("ContentID");

        builder.Property(e => e.TagId).HasColumnName("TagID");
        builder.HasOne(x => x.Tag)
           .WithMany().HasForeignKey(x => x.TagId);
    }
}