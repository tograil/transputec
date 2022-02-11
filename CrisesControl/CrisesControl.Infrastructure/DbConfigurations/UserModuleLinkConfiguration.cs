using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserModuleLinkConfiguration : IEntityTypeConfiguration<UserModuleLink>
{
    public void Configure(EntityTypeBuilder<UserModuleLink> builder)
    {
        builder.ToTable("UserModuleLInk");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.Height).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.ModuleId).HasColumnName("ModuleID");

        builder.Property(e => e.UserId).HasColumnName("UserID");

        builder.Property(e => e.Width).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.Xpos)
            .HasColumnType("decimal(18, 10)")
            .HasColumnName("XPos");

        builder.Property(e => e.Ypos)
            .HasColumnType("decimal(18, 10)")
            .HasColumnName("YPos");
    }
}