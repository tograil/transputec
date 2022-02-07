using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibSopdetailConfiguration : IEntityTypeConfiguration<LibSopdetail>
{
    public void Configure(EntityTypeBuilder<LibSopdetail> builder)
    {
        builder.ToTable("LibSOPDetail");

        builder.Property(e => e.LibSopdetailId).HasColumnName("LibSOPDetailID");

        builder.Property(e => e.LibContentId).HasColumnName("LibContentID");

        builder.Property(e => e.LibContentSectionId).HasColumnName("LibContentSectionID");

        builder.Property(e => e.LibSopheaderId).HasColumnName("LibSOPHeaderID");
    }
}