using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MergeDatumConfiguration : IEntityTypeConfiguration<MergeDatum>
{
    public void Configure(EntityTypeBuilder<MergeDatum> builder)
    {
        builder.HasNoKey();

        builder.Property(e => e.CompanyName).HasMaxLength(100);

        builder.Property(e => e.CustomerId)
            .HasMaxLength(50)
            .HasColumnName("CustomerID");

        builder.Property(e => e.Email)
            .HasMaxLength(100)
            .HasColumnName("email");
    }
}