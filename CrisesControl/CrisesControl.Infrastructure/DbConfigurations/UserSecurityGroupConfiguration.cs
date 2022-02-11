using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserSecurityGroupConfiguration : IEntityTypeConfiguration<UserSecurityGroup>
{
    public void Configure(EntityTypeBuilder<UserSecurityGroup> builder)
    {
        builder.ToTable("UserSecurityGroup");

        builder.HasIndex(e => e.SecurityGroupId, "IX_SecurityGroupId");

        builder.HasIndex(e => e.UserId, "IX_UserId");

        builder.Property(e => e.UserSecurityGroupId).HasColumnName("UserSecurityGroupID");
    }
}