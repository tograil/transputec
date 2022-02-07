using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyActivationConfiguration : IEntityTypeConfiguration<CompanyActivation>
{
    public void Configure(EntityTypeBuilder<CompanyActivation> builder)
    {
        builder.HasKey(e => e.ActivationId)
                .HasName("PK_dbo.CompanyActivation");

        builder.ToTable("CompanyActivation");

        builder.Property(e => e.ActivationId).HasColumnName("ActivationID");

        builder.Property(e => e.ActivationKey).HasMaxLength(50);

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.Ipaddress)
                .HasMaxLength(20)
                .HasColumnName("IPAddress");
    }
}