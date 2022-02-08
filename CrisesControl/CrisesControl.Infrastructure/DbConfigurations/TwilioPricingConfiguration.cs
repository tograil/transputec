using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TwilioPricingConfiguration : IEntityTypeConfiguration<TwilioPricing>
{
    public void Configure(EntityTypeBuilder<TwilioPricing> builder)
    {
        builder.ToTable("TwilioPricing");

        builder.HasIndex(e => new {e.CountryCode, e.CountryIso2}, "IDX_CountryCode_ISOCode2");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.BasePrice).HasColumnType("decimal(18, 4)");

        builder.Property(e => e.ChannelType)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.CountryCode)
            .HasMaxLength(3)
            .IsUnicode(false);

        builder.Property(e => e.CountryIso2)
            .HasMaxLength(2)
            .IsUnicode(false)
            .HasColumnName("CountryISO2");

        builder.Property(e => e.CurrentPrice).HasColumnType("decimal(18, 4)");

        builder.Property(e => e.DesinationPrefix)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.FriendlyName)
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.Property(e => e.NumberType)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}