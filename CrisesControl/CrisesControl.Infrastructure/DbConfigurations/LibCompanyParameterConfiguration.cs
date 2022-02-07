using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibCompanyParameterConfiguration : IEntityTypeConfiguration<LibCompanyParameter>
{
    public void Configure(EntityTypeBuilder<LibCompanyParameter> builder)
    {
        builder.HasKey(e => e.LibCompanyParametersId)
            .HasName("PK_dbo.LibCompanyParameters");

        builder.Property(e => e.AllowedValued).HasMaxLength(250);

        builder.Property(e => e.Description).HasMaxLength(250);

        builder.Property(e => e.Display).HasMaxLength(150);

        builder.Property(e => e.Name).HasMaxLength(70);

        builder.Property(e => e.Type).HasMaxLength(100);

        builder.Property(e => e.ValidationRule).HasMaxLength(250);

        builder.Property(e => e.Value).HasMaxLength(250);
    }
}