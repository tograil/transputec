using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class BadDomainConfiguration : IEntityTypeConfiguration<BadDomain>
{
    public void Configure(EntityTypeBuilder<BadDomain> builder)
    {
        builder.HasIndex(e => e.DomainName, "IDX_DomainName");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.DomainName)
                .HasMaxLength(100)
                .IsUnicode(false);
    }
}