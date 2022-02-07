using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SegIncidentLocationLinkConfiguration : IEntityTypeConfiguration<SegIncidentLocationLink>
{
    public void Configure(EntityTypeBuilder<SegIncidentLocationLink> builder)
    {
        builder.HasKey(e => e.IncidentLocationId)
            .HasName("PK_dbo.Seg_IncidentLocation_Link");

        builder.ToTable("Seg_IncidentLocation_Link");
    }
}