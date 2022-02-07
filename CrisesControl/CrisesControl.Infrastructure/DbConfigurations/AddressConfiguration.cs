using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Address");

        builder.Property(e => e.AddressLabel).HasMaxLength(250);

        builder.Property(e => e.AddressLine1).HasMaxLength(150);

        builder.Property(e => e.AddressLine2).HasMaxLength(150);

        builder.Property(e => e.AddressType).HasMaxLength(10);

        builder.Property(e => e.City).HasMaxLength(50);

        builder.Property(e => e.CountryCode).HasMaxLength(5);

        builder.Property(e => e.Postcode).HasMaxLength(20);

        builder.Property(e => e.State).HasMaxLength(50);
    }
}