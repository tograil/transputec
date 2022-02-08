using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SecurityGroupConfiguration : IEntityTypeConfiguration<SecurityGroup>
{
    public void Configure(EntityTypeBuilder<SecurityGroup> builder)
    {
        builder.ToTable("SecurityGroup");

        builder.HasIndex(e => e.CompanyId, "IX_CompanyId");

        builder.Property(e => e.Description).HasMaxLength(250);

        builder.Property(e => e.Name).HasMaxLength(50);

        builder.Property(e => e.UserRole).HasMaxLength(20);
    }
}