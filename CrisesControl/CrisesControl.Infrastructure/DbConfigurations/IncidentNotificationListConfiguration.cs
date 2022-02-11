using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentNotificationListConfiguration : IEntityTypeConfiguration<IncidentNotificationList>
{
    public void Configure(EntityTypeBuilder<IncidentNotificationList> builder)
    {
        builder.ToTable("IncidentNotificationList");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.IncidentActivationId, "IDX_IncidentActivationId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.ObjectMappingId, "IDX_ObjectMapId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.SourceObjectPrimaryId, "IDX_SourceObjectPrimaryId")
            .HasFillFactor(100);
    }
}