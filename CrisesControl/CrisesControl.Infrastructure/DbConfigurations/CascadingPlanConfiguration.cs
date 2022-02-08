using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CascadingPlanConfiguration : IEntityTypeConfiguration<CascadingPlan>
{
    public void Configure(EntityTypeBuilder<CascadingPlan> builder)
    {
        builder.HasKey(e => e.PlanId);

        builder.ToTable("CascadingPlan");

        builder.Property(e => e.PlanId).HasColumnName("PlanID");

        builder.Property(e => e.LaunchSos).HasColumnName("LaunchSOS");

        builder.Property(e => e.LaunchSosinterval).HasColumnName("LaunchSOSInterval");

        builder.Property(e => e.PlanName).HasMaxLength(150);

        builder.Property(e => e.PlanType).HasMaxLength(20);
    }
}