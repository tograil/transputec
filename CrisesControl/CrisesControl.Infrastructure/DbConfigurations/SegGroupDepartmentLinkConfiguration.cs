using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SegGroupDepartmentLinkConfiguration : IEntityTypeConfiguration<SegGroupDepartmentLink>
{
    public void Configure(EntityTypeBuilder<SegGroupDepartmentLink> builder)
    {
        builder.HasKey(e => e.GroupDepartmentId)
            .HasName("PK_dbo.Seg_GroupDepartment_Link");

        builder.ToTable("Seg_GroupDepartment_Link");
    }
}