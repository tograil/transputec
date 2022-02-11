using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SegGroupIncidentLinkConfiguration : IEntityTypeConfiguration<SegGroupIncidentLink>
{
    public void Configure(EntityTypeBuilder<SegGroupIncidentLink> builder)
    {
        builder.HasKey(e => e.GroupIncidentId)
            .HasName("PK_dbo.Seg_GroupIncident_Link");

        builder.ToTable("Seg_GroupIncident_Link");
    }
}