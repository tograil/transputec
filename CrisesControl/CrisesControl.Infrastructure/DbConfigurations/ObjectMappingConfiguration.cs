using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ObjectMappingConfiguration : IEntityTypeConfiguration<ObjectMapping>
{
    public void Configure(EntityTypeBuilder<ObjectMapping> builder)
    {
        builder.ToTable("ObjectMapping");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.SourceObjectId, "IDX_SouceObjectId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.TargetObjectId, "IDX_TargetObjectId")
            .HasFillFactor(100);

        builder.Property(e => e.SourceObjectId).HasColumnName("SourceObjectID");

        builder.Property(e => e.TargetObjectId).HasColumnName("TargetObjectID");
        builder.HasOne(e => e.Object).WithMany().HasForeignKey(e => e.TargetObjectId);
    }
}