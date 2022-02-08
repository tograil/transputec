using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibContentConfiguration : IEntityTypeConfiguration<LibContent>
{
    public void Configure(EntityTypeBuilder<LibContent> builder)
    {
        builder.ToTable("LibContent");

        builder.Property(e => e.LibContentId).HasColumnName("LibContentID");

        builder.Property(e => e.Checksum).HasMaxLength(40);

        builder.Property(e => e.ContentType).HasMaxLength(20);

        builder.Property(e => e.PrimaryContentId).HasColumnName("PrimaryContentID");
    }
}