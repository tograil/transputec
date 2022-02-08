using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(e => e.CountryCode)
            .HasName("PK_dbo.Countries");

        builder.ToTable("Country");

        builder.Property(e => e.CountryCode).HasMaxLength(128);

        builder.Property(e => e.CountryId).HasColumnName("CountryID");

        builder.Property(e => e.CountryPhoneCode).HasMaxLength(50);

        builder.Property(e => e.Iso2code)
            .HasMaxLength(3)
            .HasColumnName("ISO2Code");

        builder.Property(e => e.Name).HasMaxLength(100);

        builder.Property(e => e.Smsavailable).HasColumnName("SMSAvailable");

        builder.Property(e => e.SmspriceUrl)
            .HasMaxLength(200)
            .HasColumnName("SMSPriceURL");

        builder.Property(e => e.VoicePriceUrl)
            .HasMaxLength(200)
            .HasColumnName("VoicePriceURL");
    }
}