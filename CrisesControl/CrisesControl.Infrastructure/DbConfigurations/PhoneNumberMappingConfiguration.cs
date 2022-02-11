using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PhoneNumberMappingConfiguration : IEntityTypeConfiguration<PhoneNumberMapping>
{
    public void Configure(EntityTypeBuilder<PhoneNumberMapping> builder)
    {
        builder.HasKey(e => e.MappingId)
            .HasName("PK_dbo.PhoneNumberMapping");

        builder.ToTable("PhoneNumberMapping");

        builder.Property(e => e.CommsProvider)
            .HasMaxLength(15)
            .IsUnicode(false)
            .IsFixedLength();

        builder.Property(e => e.CountryDialCode).HasMaxLength(10);

        builder.Property(e => e.FromNumber).HasMaxLength(20);
    }
}