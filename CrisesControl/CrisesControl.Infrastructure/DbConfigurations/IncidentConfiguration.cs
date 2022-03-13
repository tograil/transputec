using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incident");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.Status, "IDX_Status")
            .HasFillFactor(100);

        builder.HasIndex(e => e.IncidentTypeId, "IX_IncidentTypeId");

        builder.Property(e => e.CascadePlanId).HasColumnName("CascadePlanID");

        builder.Property(e => e.IncidentIcon).HasMaxLength(250);

        builder.Property(e => e.IncidentPlanDoc).HasMaxLength(100);

        builder.Property(e => e.IsSopdoc).HasColumnName("IsSOPDoc");

        builder.Property(e => e.IsSos).HasColumnName("IsSOS");

        builder.Property(e => e.Name).HasMaxLength(150);

        builder.Property(e => e.PlanAssetId).HasColumnName("PlanAssetID");

        builder.Property(e => e.SopdocId).HasColumnName("SOPDocID");
    }
}