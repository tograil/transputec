using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ActiveIncidentAssetConfiguration : IEntityTypeConfiguration<ActiveIncidentAsset>
{
    public void Configure(EntityTypeBuilder<ActiveIncidentAsset> builder)
    {

        builder.HasNoKey();

        builder.ToTable("ActiveIncidentAsset");

        builder.Property(e => e.ActiveIncidentId).HasColumnName("ActiveIncidentID");

        builder.Property(e => e.ActiveTaskId).HasColumnName("ActiveTaskID");

        builder.Property(e => e.AssetDescription).HasMaxLength(250);

        builder.Property(e => e.AssetLinkId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AssetLinkID");

        builder.Property(e => e.AssetLinkType)
                .HasMaxLength(25)
                .IsUnicode(false);

        builder.Property(e => e.AssetPath).HasMaxLength(300);

        builder.Property(e => e.AssetTitle).HasMaxLength(250);

        builder.Property(e => e.AssetType).HasMaxLength(50);
        
    }
}