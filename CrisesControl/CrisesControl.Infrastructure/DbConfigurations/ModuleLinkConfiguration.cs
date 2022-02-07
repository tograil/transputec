using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ModuleLinkConfiguration : IEntityTypeConfiguration<ModuleLink>
{
    public void Configure(EntityTypeBuilder<ModuleLink> builder)
    {
        builder.HasKey(e => e.LinkId);

        builder.ToTable("ModuleLink");

        builder.Property(e => e.LinkId).HasColumnName("LinkID");

        builder.Property(e => e.ItemObjectId).HasColumnName("ItemObjectID");

        builder.Property(e => e.ModuleObjectId).HasColumnName("ModuleObjectID");

        builder.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");
    }
}