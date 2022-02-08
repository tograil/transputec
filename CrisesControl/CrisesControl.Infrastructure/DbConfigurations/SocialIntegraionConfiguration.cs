using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SocialIntegraionConfiguration : IEntityTypeConfiguration<SocialIntegraion>
{
    public void Configure(EntityTypeBuilder<SocialIntegraion> builder)
    {
        builder.ToTable("SocialIntegraion");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.AccountName).HasMaxLength(150);

        builder.Property(e => e.AccountType).HasMaxLength(50);

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");
    }
}