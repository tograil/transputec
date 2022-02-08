using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ContentSectionConfiguration : IEntityTypeConfiguration<ContentSection>
{
    public void Configure(EntityTypeBuilder<ContentSection> builder)
    {
        builder.ToTable("ContentSection");

        builder.Property(e => e.ContentSectionId).HasColumnName("ContentSectionID");

        builder.Property(e => e.SectionName).HasMaxLength(500);

        builder.Property(e => e.SectionType).HasMaxLength(50);

        builder.Property(e => e.SopheaderId).HasColumnName("SOPHeaderID");
    }
}