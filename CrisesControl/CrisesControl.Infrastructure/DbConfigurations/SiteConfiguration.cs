using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Site");

        builder.Property(e => e.SiteId).HasColumnName("SiteID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.SiteCode).HasMaxLength(10);

        builder.Property(e => e.SiteName).HasMaxLength(150);
    }
}