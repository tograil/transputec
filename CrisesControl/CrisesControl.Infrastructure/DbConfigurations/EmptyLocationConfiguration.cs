using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class EmptyLocationConfiguration : IEntityTypeConfiguration<EmptyLocation>
{
    public void Configure(EntityTypeBuilder<EmptyLocation> builder)
    {
        builder.HasNoKey();

        builder.ToView("Empty_Location");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.Desc).HasMaxLength(250);

        builder.Property(e => e.FirstName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.LastName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Lat).HasMaxLength(20);

        builder.Property(e => e.LocationName)
            .HasMaxLength(150)
            .HasColumnName("Location_Name");

        builder.Property(e => e.Long).HasMaxLength(20);

        builder.Property(e => e.PostCode).HasMaxLength(250);
    }
}