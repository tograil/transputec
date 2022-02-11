using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.ToTable("Content");

        builder.Property(e => e.ContentId).HasColumnName("ContentID");

        builder.Property(e => e.Checksum).HasMaxLength(40);

        builder.Property(e => e.ContentType).HasMaxLength(20);

        builder.Property(e => e.PrimaryContentId).HasColumnName("PrimaryContentID");
    }
}