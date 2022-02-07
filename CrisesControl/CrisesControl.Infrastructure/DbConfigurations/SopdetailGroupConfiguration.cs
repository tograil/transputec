using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SopdetailGroupConfiguration : IEntityTypeConfiguration<SopdetailGroup>
{
    public void Configure(EntityTypeBuilder<SopdetailGroup> builder)
    {
        builder.ToTable("SOPDetailGroup");

        builder.Property(e => e.SopdetailGroupId).HasColumnName("SOPDetailGroupID");

        builder.Property(e => e.SopdetailId).HasColumnName("SOPDetailID");

        builder.Property(e => e.SopgroupId).HasColumnName("SOPGroupID");
    }
}