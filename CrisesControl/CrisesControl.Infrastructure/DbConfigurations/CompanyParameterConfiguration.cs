using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyParameterConfiguration : IEntityTypeConfiguration<CompanyParameter>
{
    public void Configure(EntityTypeBuilder<CompanyParameter> builder)
    {
        builder.HasKey(e => e.CompanyParametersId)
            .HasName("PK_dbo.CompanyParameters");

        builder.HasIndex(e => new {e.CompanyId, e.Name}, "IDX_Name")
            .HasFillFactor(100);

        builder.Property(e => e.Name).HasMaxLength(70);

        builder.Property(e => e.Value).HasMaxLength(500);

        builder.ToTable("CompanyParameters");
    }
}