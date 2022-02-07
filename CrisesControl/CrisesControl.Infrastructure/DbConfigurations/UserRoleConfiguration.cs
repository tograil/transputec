using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(e => e.RoleId);

        builder.ToTable("UserRole");

        builder.Property(e => e.RoleId).HasColumnName("RoleID");

        builder.Property(e => e.RoleCode).HasMaxLength(20);

        builder.Property(e => e.RoleName).HasMaxLength(50);
    }
}