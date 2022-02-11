using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class GroupSecuityObjectConfiguration : IEntityTypeConfiguration<GroupSecuityObject>
{
    public void Configure(EntityTypeBuilder<GroupSecuityObject> builder)
    {
        builder.HasKey(e => e.GroupSecurityObjectId);

        builder.HasIndex(e => e.SecurityGroupId, "IX_SecurityGroupId");

        builder.HasIndex(e => e.SecurityObjectId, "IX_SecurityObjectID");

        builder.Property(e => e.GroupSecurityObjectId).HasColumnName("GroupSecurityObjectID");

        builder.Property(e => e.SecurityObjectId).HasColumnName("SecurityObjectID");
    }
}