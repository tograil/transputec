using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserRoleChangeConfiguration : IEntityTypeConfiguration<UserRoleChange>
{
    public void Configure(EntityTypeBuilder<UserRoleChange> builder)
    {
        builder.HasKey(e => e.RoleChangeId)
            .HasName("PK_dbo.UserRoleChange");

        builder.ToTable("UserRoleChange");

        builder.Property(e => e.RoleChangeId).HasColumnName("RoleChangeID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.UserId).HasColumnName("UserID");

        builder.Property(e => e.UserRole).HasMaxLength(20);
    }
}