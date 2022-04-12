using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SecurityObjectConfiguration : IEntityTypeConfiguration<SecurityObject>
{
    public void Configure(EntityTypeBuilder<SecurityObject> builder)
    {
        builder.HasIndex(e => e.Status, "IDX_Status")
            .HasFillFactor(100);

        builder.Property(e => e.SecurityObjectId).HasColumnName("SecurityObjectID");

        builder.Property(e => e.Description).HasMaxLength(150);

        builder.Property(e => e.MenuOrder).HasColumnType("decimal(18, 0)");

        builder.Property(e => e.Name).HasMaxLength(50);

        builder.Property(e => e.ParentId).HasColumnName("ParentID");

        builder.Property(e => e.RoleId).HasColumnName("RoleID");

        builder.Property(e => e.SecurityKey).HasMaxLength(150);

        builder.Property(e => e.Target).HasMaxLength(150);

        builder.Property(e => e.TypeId).HasColumnName("TypeID");

        builder.ToTable("SecurityObjects");
    }
}