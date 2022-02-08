using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.Property(e => e.ModuleId).HasColumnName("ModuleID");

        builder.Property(e => e.LinkId).HasColumnName("LinkID");

        builder.Property(e => e.ModuleChargeType)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.ModuleName)
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(e => e.ModuleType)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.ParameterKey)
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(e => e.ParentId).HasColumnName("ParentID");

        builder.Property(e => e.ProductCode)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.SecurityObjectId).HasColumnName("SecurityObjectID");

        builder.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");
    }
}