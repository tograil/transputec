using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CrisesControl.Core.Incidents;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentActivationConfiguration : IEntityTypeConfiguration<IncidentActivation>
{
    public void Configure(EntityTypeBuilder<IncidentActivation> builder)
    {
        builder.ToTable("IncidentActivation");

        builder.HasIndex(e => e.ImpactedLocationId, "IDX_ImpactedLocationId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.IncidentId, "IDX_IncidentId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.LaunchedBy, "IDX_LaunchedBy");

        builder.HasIndex(e => e.Status, "IDX_Status");

        builder.HasIndex(e => new {e.IncidentId, e.CompanyId}, "IDX_companyID");

        builder.Property(e => e.CascadePlanId).HasColumnName("CascadePlanID");

        builder.Property(e => e.IncidentIcon).HasMaxLength(250);

        builder.Property(e => e.Name).HasMaxLength(150);

        builder.Property(e => e.PlanAssetId).HasColumnName("PlanAssetID");

        builder.Property(e => e.SocialHandle)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.HasOne(x => x.Incident).WithMany().HasForeignKey(x => x.IncidentId);
    }
}