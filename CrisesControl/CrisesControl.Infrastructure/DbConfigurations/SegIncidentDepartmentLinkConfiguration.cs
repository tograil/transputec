using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SegIncidentDepartmentLinkConfiguration : IEntityTypeConfiguration<SegIncidentDepartmentLink>
{
    public void Configure(EntityTypeBuilder<SegIncidentDepartmentLink> builder)
    {
        builder.HasKey(e => e.IncidentDepartmentId)
            .HasName("PK_dbo.Seg_IncidentDepartment_Link");

        builder.ToTable("Seg_IncidentDepartment_Link");
    }
}