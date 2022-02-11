using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibContentTagConfiguration : IEntityTypeConfiguration<LibContentTag>
{
    public void Configure(EntityTypeBuilder<LibContentTag> builder)
    {
        builder.ToTable("LibContentTag");

        builder.Property(e => e.LibContentTagId).HasColumnName("LibContentTagID");

        builder.Property(e => e.LibContentId).HasColumnName("LibContentID");

        builder.Property(e => e.TagId).HasColumnName("TagID");
    }
}