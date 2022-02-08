using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SopdetailConfiguration : IEntityTypeConfiguration<Sopdetail>
{
    public void Configure(EntityTypeBuilder<Sopdetail> builder)
    {
        builder.ToTable("SOPDetail");

        builder.Property(e => e.SopdetailId).HasColumnName("SOPDetailID");

        builder.Property(e => e.ContentId).HasColumnName("ContentID");

        builder.Property(e => e.ContentSectionId).HasColumnName("ContentSectionID");

        builder.Property(e => e.SopheaderId).HasColumnName("SOPHeaderID");
    }
}