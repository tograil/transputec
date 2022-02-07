using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class DashboardModuleConfiguration : IEntityTypeConfiguration<DashboardModule>
{
    public void Configure(EntityTypeBuilder<DashboardModule> builder)
    {
        builder.HasKey(e => e.ModuleId);

        builder.ToTable("DashboardModule");

        builder.Property(e => e.ModuleId).HasColumnName("ModuleID");

        builder.Property(e => e.ColorScheme)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.ContainerId)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("ContainerID");

        builder.Property(e => e.Height).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.MaxHeight).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.MaxWidth).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.MinHeight).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.MinWidth).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.ModuleName)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.ModulePage)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.ResizeHandle)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.WidgetFilePath)
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.Property(e => e.Width).HasColumnType("decimal(18, 10)");

        builder.Property(e => e.Xpos)
            .HasColumnType("decimal(18, 10)")
            .HasColumnName("XPos");

        builder.Property(e => e.Ypos)
            .HasColumnType("decimal(18, 10)")
            .HasColumnName("YPos");
    }
}