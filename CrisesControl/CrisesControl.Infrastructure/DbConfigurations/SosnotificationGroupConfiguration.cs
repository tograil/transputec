using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SosnotificationGroupConfiguration : IEntityTypeConfiguration<SosnotificationGroup>
{
    public void Configure(EntityTypeBuilder<SosnotificationGroup> builder)
    {
        builder.HasKey(e => e.SosnotificationId)
            .HasName("PK_dbo.SOSNotificationGroup");

        builder.ToTable("SOSNotificationGroup");

        builder.Property(e => e.SosnotificationId).HasColumnName("SOSNotificationID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");

        builder.Property(e => e.ObjectMappingId).HasColumnName("ObjectMappingID");

        builder.Property(e => e.SourceObjectPrimaryId).HasColumnName("SourceObjectPrimaryID");
    }
}