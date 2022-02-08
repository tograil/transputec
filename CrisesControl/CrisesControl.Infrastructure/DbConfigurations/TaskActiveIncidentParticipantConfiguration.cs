using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskActiveIncidentParticipantConfiguration : IEntityTypeConfiguration<TaskActiveIncidentParticipant>
{
    public void Configure(EntityTypeBuilder<TaskActiveIncidentParticipant> builder)
    {
        builder.HasKey(e => e.ActiveIncidentTaskParticipantId)
            .HasName("PK_dbo.TaskActiveIncidentParticipant");

        builder.ToTable("TaskActiveIncidentParticipant");

        builder.Property(e => e.ActiveIncidentTaskParticipantId).HasColumnName("ActiveIncidentTaskParticipantID");

        builder.Property(e => e.ActionStatus).HasMaxLength(20);

        builder.Property(e => e.ActiveIncidentTaskId).HasColumnName("ActiveIncidentTaskID");

        builder.Property(e => e.ParticipantGroupId).HasColumnName("ParticipantGroupID");

        builder.Property(e => e.ParticipantTypeId).HasColumnName("ParticipantTypeID");

        builder.Property(e => e.ParticipantUserId).HasColumnName("ParticipantUserID");

        builder.Property(e => e.PreviousParticipantTypeId).HasColumnName("PreviousParticipantTypeID");
    }
}