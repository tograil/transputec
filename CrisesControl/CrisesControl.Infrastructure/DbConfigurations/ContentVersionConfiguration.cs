using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ContentVersionConfiguration : IEntityTypeConfiguration<ContentVersion>
{
    public void Configure(EntityTypeBuilder<ContentVersion> builder)
    {
        builder.ToTable("ContentVersion");

        builder.Property(e => e.ContentVersionId).HasColumnName("ContentVersionID");

        builder.Property(e => e.LastContentId).HasColumnName("LastContentID");

        builder.Property(e => e.PrimaryContentId).HasColumnName("PrimaryContentID");
    }
}