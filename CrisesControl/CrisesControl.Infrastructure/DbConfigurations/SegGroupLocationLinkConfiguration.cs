using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SegGroupLocationLinkConfiguration : IEntityTypeConfiguration<SegGroupLocationLink>
{
    public void Configure(EntityTypeBuilder<SegGroupLocationLink> builder)
    {
        builder.HasKey(e => e.GroupLocationId)
                .HasName("PK_dbo.Seg_GroupLocation_Link");

        builder.ToTable("Seg_GroupLocation_Link");
    }
}