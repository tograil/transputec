using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ChargeableFeatureConfiguration : IEntityTypeConfiguration<ChargeableFeature>
{
    public void Configure(EntityTypeBuilder<ChargeableFeature> builder)
    {
        builder.HasKey(e => e.LinkId)
                .HasName("PK_dbo.ChargeableFeature");

        builder.ToTable("ChargeableFeature");

        builder.Property(e => e.LinkId).HasColumnName("LinkID");

        builder.Property(e => e.SecurityObjectId).HasColumnName("SecurityObjectID");

        builder.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");
    }
}