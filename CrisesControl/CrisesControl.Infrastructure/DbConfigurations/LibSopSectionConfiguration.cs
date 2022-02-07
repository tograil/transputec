using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibSopSectionConfiguration : IEntityTypeConfiguration<LibSopSection>
{
    public void Configure(EntityTypeBuilder<LibSopSection> builder)
    {
        builder.Property(e => e.LibSopSectionId).HasColumnName("LibSopSectionID");

        builder.Property(e => e.SectionName).HasMaxLength(250);

        builder.Property(e => e.SectionType).HasMaxLength(20);
    }
}