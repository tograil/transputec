using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskIncidentParticipantConfiguration : IEntityTypeConfiguration<TaskIncidentParticipant>
{
    public void Configure(EntityTypeBuilder<TaskIncidentParticipant> builder)
    {
        builder.HasKey(e => e.IncidentTaskParticipantId)
            .HasName("PK_dbo.TaskIncidentParticipant");

        builder.ToTable("TaskIncidentParticipant");

        builder.HasIndex(e => e.IncidentTaskId, "IDX_IncidentTaskID");

        builder.HasIndex(e => e.ParticipantTypeId, "IDX_ParticipantTypeID");

        builder.HasIndex(e => e.ParticipantUserId, "IDX_ParticipantUserID");

        builder.Property(e => e.IncidentTaskParticipantId).HasColumnName("IncidentTaskParticipantID");

        builder.Property(e => e.IncidentTaskId).HasColumnName("IncidentTaskID");

        builder.Property(e => e.ParticipantGroupId).HasColumnName("ParticipantGroupID");

        builder.Property(e => e.ParticipantTypeId).HasColumnName("ParticipantTypeID");

        builder.Property(e => e.ParticipantUserId).HasColumnName("ParticipantUserID");
    }
}