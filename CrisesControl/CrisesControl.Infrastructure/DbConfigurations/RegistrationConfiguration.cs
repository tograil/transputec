using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.AddressLine1).HasMaxLength(150);

        builder.Property(e => e.AddressLine2).HasMaxLength(150);

        builder.Property(e => e.City).HasMaxLength(100);

        builder.Property(e => e.CompanyName).HasMaxLength(200);

        builder.Property(e => e.CountryCode)
            .HasMaxLength(5)
            .IsUnicode(false);

        builder.Property(e => e.CustomerId).HasMaxLength(100);

        builder.Property(e => e.Email).HasMaxLength(150);

        builder.Property(e => e.FirstName).HasMaxLength(50);

        builder.Property(e => e.LastName).HasMaxLength(50);

        builder.Property(e => e.MobileIsd)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasColumnName("MobileISD");

        builder.Property(e => e.MobileNo)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.Password).HasMaxLength(50);

        builder.Property(e => e.PaymentMethod)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.Postcode)
            .HasMaxLength(25)
            .IsUnicode(false);

        builder.Property(e => e.Sector).HasMaxLength(150);

        builder.Property(e => e.State).HasMaxLength(100);

        builder.Property(e => e.UniqueReference)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.VerificationCode)
            .HasMaxLength(10)
            .IsUnicode(false);
    }
}