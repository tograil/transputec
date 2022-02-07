using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyCommConfiguration : IEntityTypeConfiguration<CompanyComm>
{
    public void Configure(EntityTypeBuilder<CompanyComm> builder)
    {
        builder.HasKey(e => e.CompanyCommsId)
                .HasName("PK_dbo.CompanyComms");
    }
}