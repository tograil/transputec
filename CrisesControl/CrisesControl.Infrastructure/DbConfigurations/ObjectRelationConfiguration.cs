using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ObjectRelationConfiguration : IEntityTypeConfiguration<ObjectRelation>
{
    public void Configure(EntityTypeBuilder<ObjectRelation> builder)
    {
        builder.ToTable("ObjectRelation");

        builder.HasIndex(e => e.ObjectMappingId, "IDX_ObjectMappingId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.SourceObjectPrimaryId, "IDX_SourceObjectPrimaryId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.TargetObjectPrimaryId, "IDX_TargetObjectPrimaryId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.ObjectMappingId, "IDX_Target_Source_ID");

        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.TargetObjectPrimaryId);
    }
}