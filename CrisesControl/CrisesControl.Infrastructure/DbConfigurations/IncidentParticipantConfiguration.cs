using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentParticipantConfiguration : IEntityTypeConfiguration<IncidentParticipant>
{
    public void Configure(EntityTypeBuilder<IncidentParticipant> builder)
    {
        builder.ToTable("IncidentParticipant");

        builder.Property(e => e.IncidentParticipantId).HasColumnName("IncidentParticipantID");

        builder.Property(e => e.IncidentActionId).HasColumnName("IncidentActionID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");

        builder.Property(e => e.ObjectMappingId).HasColumnName("ObjectMappingID");

        builder.Property(e => e.ParticipantGroupId).HasColumnName("ParticipantGroupID");

        builder.Property(e => e.ParticipantType)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.ParticipantUserId).HasColumnName("ParticipantUserID");
    }
}