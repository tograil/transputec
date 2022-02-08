using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ActiveIncidentActionConfiguration : IEntityTypeConfiguration<ActiveIncidentAction>
{
    public void Configure(EntityTypeBuilder<ActiveIncidentAction> builder)
    {
        builder.ToTable("ActiveIncidentAction");

        builder.HasIndex(e => e.CompanyId, "IX_CompanyId");
    }
}