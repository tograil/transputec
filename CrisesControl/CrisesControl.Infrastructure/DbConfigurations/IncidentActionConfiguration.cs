using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentActionConfiguration : IEntityTypeConfiguration<IncidentAction>
{
    public void Configure(EntityTypeBuilder<IncidentAction> builder)
    {
        builder.ToTable("IncidentAction");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.Status, "IDX_Status")
            .HasFillFactor(100);

        builder.HasIndex(e => e.IncidentId, "IX_IncidentId");

        builder.Property(e => e.Title).HasMaxLength(150);
    }
}