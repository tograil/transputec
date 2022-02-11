using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibContentSectionConfiguration : IEntityTypeConfiguration<LibContentSection>
{
    public void Configure(EntityTypeBuilder<LibContentSection> builder)
    {
        builder.HasKey(e => e.LibSectionId)
            .HasName("PK_dbo.LibContentSection");

        builder.ToTable("LibContentSection");

        builder.Property(e => e.LibSectionId).HasColumnName("LibSectionID");

        builder.Property(e => e.LibSopheaderId).HasColumnName("LibSOPHeaderID");

        builder.Property(e => e.SectionName).HasMaxLength(500);
    }
}